using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;
using KOSM.Utility;

namespace KOSM.Game
{
    public class Rocket : WorldObject, IRocket
    {
        internal Vessel raw = null;

        public Rocket(World world, Vessel vessel)
            : base(world)
        {
            this.raw = vessel;
        }

        #region WorldObject

        public override string Identifier
        {
            get { return raw.id.ToString(); }
        }

        #endregion WorldObject

        #region IRocket

        public string Name
        {
            get { return raw.GetName(); }
        }

        public IBody Body
        {
            get { return world.FindBodyByName(raw.mainBody.GetName()); }
        }

        public IOrbit Orbit
        {
            get { return new Orbit(world, raw.GetOrbit(), this.Body); }
        }

        public double RocketVerticalHeight
        {
            get
            {
                if (raw.rigidbody == null)
                    return 0;

                double altitudeOfPart = double.MaxValue;
                foreach (Part part in raw.Parts)
                {
                    if (part.collider == null)
                        continue;

                    UnityEngine.Vector3 extents = part.collider.bounds.extents;
                    float partRadius = Math.Max(extents[0], Math.Max(extents[1], extents[2]));
                    double partAltitudeBottom = Math.Max(0, raw.mainBody.GetAltitude(part.collider.bounds.center) - partRadius);
                    if (partAltitudeBottom < altitudeOfPart)
                        altitudeOfPart = partAltitudeBottom;
                }

                return Altitude - altitudeOfPart;
            }
        }

        public List<IManeuver> Maneuvers
        {
            get { return raw.patchedConicSolver.maneuverNodes.Select(a => new Maneuver(world, this, raw.patchedConicSolver, a) as IManeuver).ToList(); }
        }

        public IManeuver NextManeuver
        {
            get
            {
                if (Maneuvers.Count == 0)
                    return null;

                var timeOfNextManeuver = Maneuvers.Where(a => a.TimeWhenDue > world.PointInTime).Min(a => a.TimeWhenDue);
                return Maneuvers.Where(a => a.TimeWhenDue == timeOfNextManeuver).First();
            }
        }

        public void AddApoapsisManeuver(double targetRadius)
        {
            addNode(world.PointInTime + this.Orbit.TimeToApoapsis, this.Orbit.DeltaVForApoapsisManeuver(targetRadius));
        }

        public void AddPeriapsisManeuver(double targetRadius)
        {
            addNode(world.PointInTime + this.Orbit.TimeToPeriapsis, this.Orbit.DeltaVForPeriapsisManeuver(targetRadius));
        }

        public void AddHohmannManeuver(IBody targetBody)
        {
            throw new NotImplementedException();

            // this.MainBody.raw.Orbit;
            // this.MainBody.raw.OrbitDriver.OnRailsSOITransition();
            // OrbitUtil.GetTransferTime(o.referenceBody.orbit, destination);
        }

        public void AddInclinationChangeManeuver(IBody targetBody)
        {
            if (!targetBody.IsOrbiting(Body) && !Body.IsOrbiting(targetBody))
                throw new Exception("For inclination changes, one of the two bodies must orbit the other!");

            throw new NotImplementedException();
            // Vector3d targetVelocity = targetBody.IsOrbiting(Body) ? targetBody.Orbit.CurrentVelocity : -1 * Body.Orbit.CurrentVelocity;
            // Vector3d angularMomentum = Vector3d.Cross(targetBody.Position - targetBody.Body.Position, targetVelocity);
            // HERE Vector3d angularMomentumRocket = Vector3d.Cross(this.Position - this.MainBody.Position);

        }

        public double Throttle
        {
            get
            {
                return raw == FlightGlobals.ActiveVessel ? FlightInputHandler.state.mainThrottle : raw.ctrlState.mainThrottle;
            }
            set
            {
                value = clamp(value, 0, 1);

                if (value > 0)
                {
                    world.PreventTimeWarping();
                    checkStaging();
                }

                raw.ctrlState.mainThrottle = (float)value;

                if (raw == FlightGlobals.ActiveVessel)
                    FlightInputHandler.state.mainThrottle = (float)value;
            }
        }

        public void LockHeading(IEulerAngles3 newHeading)
        {
            lockHeading(UnityEngine.Quaternion.Euler(newHeading.X, newHeading.Y, newHeading.Z));
        }

        /// <summary>
        /// Wish for a target heading, pitch and roll. We see what we can do.
        /// </summary>
        /// <param name="pitchAboveHorizon">-90° (down) to 90° up</param>
        /// <param name="degreesFromNorth">0 to 360° (North = 0°, East = 90°, South = 180°, West = 270°)</param>
        /// <param name="roll">--180° to 180°</param>
        public void SetCompassSteering(double pitchAboveHorizon, double degreesFromNorth, double roll)
        {
            UnityEngine.Quaternion targetDir = UnityEngine.Quaternion.LookRotation(getNorth(), raw.upAxis);
            targetDir *= UnityEngine.Quaternion.Euler(new UnityEngine.Vector3((float)-pitchAboveHorizon, (float)degreesFromNorth, (float)roll));
            lockHeading(targetDir);
        }

        public void SetSteering(IVector3 forward)
        {
            lockHeading(UnityEngine.Quaternion.LookRotation(v3d(forward), facing * UnityEngine.Vector3.up));
        }

        public void Stage()
        {
            raw.MakeActive();

            if (!raw.isActiveVessel)
                throw new Exception("Vessel has to be active to activate next stage.");

            if (Staging.separate_ready)
                Staging.ActivateNextStage();
        }

        public void RaiseGear()
        {
            raw.ActionGroups.SetGroup(KSPActionGroup.Gear, false);
        }

        public void LowerGear()
        {
            raw.ActionGroups.SetGroup(KSPActionGroup.Gear, true);
        }

        public double MissionTime
        {
            get { return raw.missionTime; }
        }

        public bool Landed
        {
            get { return raw.situation == Vessel.Situations.LANDED || raw.situation == Vessel.Situations.PRELAUNCH || raw.situation == Vessel.Situations.SPLASHED; }
        }

        public bool InCircularOrbit
        {
            get { return raw.situation == Vessel.Situations.ORBITING && Orbit.IsCircular; }
        }

        public double Altitude
        {
            get { return raw.altitude; }
        }

        public double AltitudeOverGround
        {
            get { return raw.GetHeightFromTerrain() - RocketVerticalHeight; }
        }

        public bool Turned
        {
            get { return TurnDeviation < 0.05;  } // less than 0.05° deviation }
        }

        public double TurnDeviation
        {
            get
            {
                double vdot = Vector3d.Dot(this.raw.Autopilot.SAS.lockedHeading.eulerAngles.normalized, this.raw.Autopilot.SAS.currentRotation.eulerAngles.normalized);
                return vdot >= 1 ? 0 : vdot <= -1 ? 180 : 180 * Math.Acos(vdot) / Math.PI;
            }
        }

        /// <summary>
        /// 0 to 360° (North = 0°, East = 90°, South = 180°, West = 270°)
        /// </summary>
        public double Heading
        {
            get { return rotationVesselSurface.eulerAngles.y; }
        }

        /// <summary>
        /// -90° (down) to 90° (up)
        /// </summary>
        public double Pitch
        {
            get { return pitchEulerToNavBall(rotationVesselSurface.eulerAngles.x); }
        }

        /// <summary>
        /// -180° to 180°
        /// </summary>
        public double Roll
        {
            get { return rollEulerToNavBall(rotationVesselSurface.eulerAngles.z); }
        }

        public IVector3 Up
        {
            get { return v3(raw.upAxis); }
        }

        public IVector3 OrbitRetrograde
        {
            get { return v3(raw.obt_velocity.normalized * -1); }
        }

        public IVector3 SurfaceRetrograde
        {
            get { return v3(raw.srf_velocity.normalized * -1); }
        }

        public double MaxAcceleration
        {
            get { return this.raw.GetTotalMass() == 0 ? 0 : this.MaxThrust / this.raw.GetTotalMass(); }
        }

        public double CurrentThrust
        {
            get
            {
                double thrust = 0.0;

                foreach (Part part in raw.parts)
                    foreach (PartModule partModule in part.Modules)
                        if (partModule.isEnabled && partModule is ModuleEngines)
                            thrust += engineThrust((ModuleEngines)partModule, Throttle);

                return thrust;
            }
        }

        public double MaxThrust
        {
            get
            {
                double thrust = 0.0;

                foreach (Part part in raw.parts)
                    foreach (PartModule partModule in part.Modules)
                        if (partModule.isEnabled && partModule is ModuleEngines)
                            thrust += engineThrust((ModuleEngines)partModule, 1.0f);

                return thrust;
            }
        }

        public double VerticalSpeed
        {
            get { return raw.verticalSpeed; }
        }

        public double HorizontalSurfaceSpeed
        {
            get { return raw.horizontalSrfSpeed; }
        }

        public IVector3 SurfaceVelocity
        {
            get { return v3(raw.srf_velocity); }
        }

        public double TimeToMissionTime(double pointInTime)
        {
            return MissionTime + pointInTime - world.PointInTime;
        }

        #endregion IRocket

        private void addNode(double pointInTime, IVector3 deltaV)
        {
            ManeuverNode node = raw.patchedConicSolver.AddManeuverNode(pointInTime);
            node.DeltaV = v3d(deltaV);
            raw.patchedConicSolver.UpdateFlightPlan();
        }

        private void addNode(double pointInTime, double radialOut, double normal, double prograde)
        {
            ManeuverNode node = raw.patchedConicSolver.AddManeuverNode(pointInTime);
            node.DeltaV = new Vector3d(radialOut, normal, prograde);
            raw.patchedConicSolver.UpdateFlightPlan();
        }

        private UnityEngine.Quaternion rotationVesselSurface
        {
            get
            {
                UnityEngine.Quaternion rotationSurface = UnityEngine.Quaternion.LookRotation((UnityEngine.Vector3)getNorth(), (UnityEngine.Vector3)getUp());
                return UnityEngine.Quaternion.Inverse(UnityEngine.Quaternion.Euler(90, 0, 0) * UnityEngine.Quaternion.Inverse(raw.GetTransform().rotation) * rotationSurface);
            }
        }

        private void checkStaging()
        {
            if (MaxThrust > 0)
                return;

            Stage();
        }

        private double engineThrust(ModuleEngines engine, double throttle)
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
            return Scalar.Interpolate(engine.minFuelFlow, engine.maxFuelFlow, throttle) * flowMod * engine.atmosphereCurve.Evaluate((float)atmPressure) * engine.g * velMod;
        }

        private double pitchEulerToNavBall(double value)
        {
            return value > 180 ? 360.0 - value : -value;
        }

        private double rollEulerToNavBall(double value)
        {
            return value > 180 ? value - 360.0 : value;
        }

        private void enableStabilityAssist()
        {
            if (!raw.ActionGroups[KSPActionGroup.SAS])
                raw.ActionGroups.SetGroup(KSPActionGroup.SAS, true);
            if (!raw.ActionGroups[KSPActionGroup.RCS])
                raw.ActionGroups.SetGroup(KSPActionGroup.RCS, true);

            if (this.raw.Autopilot.Mode != VesselAutopilot.AutopilotMode.StabilityAssist)
                this.raw.Autopilot.SetMode(VesselAutopilot.AutopilotMode.StabilityAssist);
        }

        private Vector3d getNorth()
        {
            return Vector3d.Exclude(getUp(), (raw.mainBody.position + toDouble(raw.mainBody.transform.up) * raw.mainBody.Radius) - raw.findWorldCenterOfMass()).normalized;
        }

        private Vector3d toDouble(UnityEngine.Vector3 value)
        {
            return new Vector3d(value.x, value.y, value.z);
        }

        private Vector3d getUp()
        {
            return (raw.findWorldCenterOfMass() - raw.mainBody.position).normalized;
        }

        private void lockHeading(UnityEngine.Quaternion newHeading)
        {
            newHeading = newHeading * UnityEngine.Quaternion.Euler(90, 0, 0);

            enableStabilityAssist();

            if (raw.Autopilot.SAS.lockedHeading != newHeading)
                this.raw.Autopilot.SAS.LockHeading(newHeading, true);

            if (!Turned)
                world.PreventTimeWarping();
        }

        private UnityEngine.Quaternion facing
        {
            get { return UnityEngine.Quaternion.Inverse(UnityEngine.Quaternion.Euler(90, 0, 0) * UnityEngine.Quaternion.Inverse(raw.ReferenceTransform.rotation) * UnityEngine.Quaternion.identity); }
        }
    }
}
