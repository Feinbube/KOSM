using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KOSM.Game
{
    public class Rocket : WorldObject
    {
        private Vessel vessel = null;
        private Orbit orbit = null;

        public string Name { get { return vessel.GetName(); } }

        public double MissionTime { get { return vessel.missionTime; } }

        public double Altitude { get { return vessel.altitude; } }
        public double AltitudeOverGround { get { return vessel.terrainAltitude; } }

        public Body MainBody { get { return world.FindBodyByName(vessel.mainBody.GetName()); } }

        public Orbit Orbit
        {
            get
            {
                if (orbit == null)
                    orbit = new Orbit(world, vessel.GetOrbit());
                return orbit;
            }
        }

        public Rocket(World world, Vessel vessel)
            : base(world)
        {
            this.vessel = vessel;
        }

        #region Thrust ############################################################################

        public double Throttle
        {
            get { return vessel == FlightGlobals.ActiveVessel ? FlightInputHandler.state.mainThrottle : vessel.ctrlState.mainThrottle; }
            set
            {
                value = clamp(value, 0, 1);

                if (value > 0)
                    checkStaging();

                vessel.ctrlState.mainThrottle = (float)value;

                if (vessel == FlightGlobals.ActiveVessel)
                    FlightInputHandler.state.mainThrottle = (float)value;
            }
        }

        private void checkStaging()
        {
            if (MaxThrust > 0)
                return;

            Stage();
        }

        public void Stage()
        {
            vessel.MakeActive();

            if (!vessel.isActiveVessel)
                throw new Exception("Vessel has to be active to activate next stage.");

            if (Staging.separate_ready)
                Staging.ActivateNextStage();
        }

        public double MaxThrust
        {
            get
            {
                double thrust = 0.0;

                foreach (Part part in vessel.parts)
                    foreach (PartModule partModule in part.Modules)
                        if (partModule.isEnabled && partModule is ModuleEngines)
                            thrust += EngineThrust((ModuleEngines)partModule, 1.0f);

                return thrust;
            }
        }

        public double CurrentThrust
        {
            get
            {
                double thrust = 0.0;

                foreach (Part part in vessel.parts)
                    foreach (PartModule partModule in part.Modules)
                        if (partModule.isEnabled && partModule is ModuleEngines)
                            thrust += EngineThrust((ModuleEngines)partModule, vessel.ctrlState.mainThrottle);

                return thrust;
            }
        }
        public float EngineThrust(ModuleEngines engine, float throttle)
        {
            if (engine == null || !engine.isOperational)
                return 0.0f;

            throttle = throttle * engine.thrustPercentage / 100.0f; // consider thrust limiters
            double atmPressure = engine.part.staticPressureAtm;

            float flowMod = 1.0f;
            float velMod = 1.0f;
            if (engine.atmChangeFlow)
                flowMod = (float)(engine.part.atmDensity / 1.225f);

            if (engine.useAtmCurve && engine.atmCurve != null)
                flowMod = engine.atmCurve.Evaluate(flowMod);

            if (engine.useVelCurve && engine.velCurve != null)
                velMod = velMod * engine.velCurve.Evaluate((float)engine.vessel.mach);

            // thrust = (modified fuel flow rate) * isp * g * (velocity modifier for jet engines) // KSP 1.0
            return Mathf.Lerp(engine.minFuelFlow, engine.maxFuelFlow, throttle) * flowMod * engine.atmosphereCurve.Evaluate((float)atmPressure) * engine.g * velMod;
        }

        public double MaxAcceleration
        {
            get
            {
                return this.vessel.GetTotalMass() == 0 ? 0 : this.MaxThrust / this.vessel.GetTotalMass();
            }
        }

        #endregion Thrust  ########################################################################

        #region Rotation  #########################################################################

        private Quaternion rotationVesselSurface
        {
            get
            {
                Quaternion rotationSurface = Quaternion.LookRotation((Vector3)getNorth(), (Vector3)getUp());
                return Quaternion.Inverse(Quaternion.Euler(90, 0, 0) * Quaternion.Inverse(vessel.GetTransform().rotation) * rotationSurface);
            }
        }

        /// <summary>
        /// 0 to 360° (North = 0°, East = 90°, South = 180°, West = 270°)
        /// </summary>
        public double Heading { get { return rotationVesselSurface.eulerAngles.y; } }

        /// <summary>
        /// -90° (down) to 90° (up)
        /// </summary>
        public double Pitch { get { return pitchEulerToNavBall(rotationVesselSurface.eulerAngles.x); } }

        private double pitchNavBallEuler(double value)
        {
            return value > 0 ? -value - 360.0 : -value;
        }

        private double pitchEulerToNavBall(double value)
        {
            return value > 180 ? 360.0 - value : -value;
        }

        /// <summary>
        /// -180° to 180°
        /// </summary>
        public double Roll { get { return rollEulerToNavBall(rotationVesselSurface.eulerAngles.z); } }

        private double rollNavBallEuler(double value)
        {
            return value < 0 ? value + 360.0 : value;
        }

        private double rollEulerToNavBall(double value)
        {
            return value > 180 ? value - 360.0 : value;
        }

        public bool Turned
        {
            get
            {
                return (this.vessel.Autopilot.SAS.lockedHeading.eulerAngles - this.vessel.Autopilot.SAS.currentRotation.eulerAngles).magnitude < 0.5;
            }
        }

        /// <summary>
        /// Wish for a target heading, pitch and roll. We see what we can do.
        /// </summary>
        /// <param name="pitchAboveHorizon">-90° (down) to 90° up</param>
        /// <param name="degreesFromNorth">0 to 360° (North = 0°, East = 90°, South = 180°, West = 270°)</param>
        /// <param name="roll">--180° to 180°</param>
        public void SetCompassSteering(double pitchAboveHorizon, double degreesFromNorth, double roll)
        {
            Quaternion targetDir = Quaternion.LookRotation(getNorth(), vessel.upAxis);
            targetDir *= Quaternion.Euler(new Vector3((float)-pitchAboveHorizon, (float)degreesFromNorth, (float)roll));
            LockHeading(targetDir);
        }

        public void SetSteering(Vector3d forward)
        {
            LockHeading(Quaternion.LookRotation(forward, Facing * Vector3.up));
        }

        public Quaternion Facing
        {
            get
            {
                return Quaternion.Inverse(Quaternion.Euler(90, 0, 0) * Quaternion.Inverse(vessel.ReferenceTransform.rotation) * Quaternion.identity);
            }
        }

        public void LockHeading(Quaternion newHeading)
        {
            newHeading = newHeading * Quaternion.Euler(90, 0, 0);

            enableStabilityAssist();

            if (vessel.Autopilot.SAS.lockedHeading != newHeading)
                this.vessel.Autopilot.SAS.LockHeading(newHeading, true);
        }

        private void enableStabilityAssist()
        {
            if (!vessel.ActionGroups[KSPActionGroup.SAS])
                vessel.ActionGroups.SetGroup(KSPActionGroup.SAS, true);
            if (!vessel.ActionGroups[KSPActionGroup.RCS])
                vessel.ActionGroups.SetGroup(KSPActionGroup.RCS, true);

            if (this.vessel.Autopilot.Mode != VesselAutopilot.AutopilotMode.StabilityAssist)
                this.vessel.Autopilot.SetMode(VesselAutopilot.AutopilotMode.StabilityAssist);
        }

        private Vector3d getNorth()
        {
            return Vector3d.Exclude(getUp(), (vessel.mainBody.position + toDouble(vessel.mainBody.transform.up) * vessel.mainBody.Radius) - vessel.findWorldCenterOfMass()).normalized;
        }

        private Vector3d toDouble(Vector3 value)
        {
            return new Vector3d(value.x, value.y, value.z);
        }

        private Vector3d getUp()
        {
            return (vessel.findWorldCenterOfMass() - vessel.mainBody.position).normalized;
        }

        #endregion Rotation #######################################################################

        #region Maneuvers  ########################################################################

        public List<Maneuver> Maneuvers { get { return vessel.patchedConicSolver.maneuverNodes.Select(a => new Maneuver(world, this, vessel.patchedConicSolver, a)).ToList(); } }

        public Maneuver NextManeuver
        {
            get
            {
                var timeOfNextManeuver = Maneuvers.Where(a => a.DueTime > world.PointInTime).Min(a => a.DueTime);
                return Maneuvers.Where(a => a.DueTime == timeOfNextManeuver).First();
            }
        }

        public void AddApoapsisManeuver(double targetAltitude)
        {
            addNode(world.PointInTime + this.orbit.TimeToApoapsis, 0, 0, this.orbit.DeltaVForApoapsisManeuver(targetAltitude));
        }

        private void addNode(double pointInTime, double radialOut, double normal, double prograde)
        {
            ManeuverNode node = vessel.patchedConicSolver.AddManeuverNode(pointInTime);
            node.DeltaV = new Vector3d(radialOut, normal, prograde);
            vessel.patchedConicSolver.UpdateFlightPlan();
        }

        #endregion Maneuvers  #####################################################################

        public override bool Equals(object obj)
        {
            return vessel.id.Equals((obj as Rocket).vessel.id);
        }

        public override int GetHashCode()
        {
            return vessel.id.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}