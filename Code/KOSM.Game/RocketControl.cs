using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using KOSM.Common;
using KOSM.Interfaces;
using KOSM.Utility;

namespace KOSM.Game
{
    public class RocketControl : WorldObject, IRocketControl
    {
        private Rocket rocket;

        public RocketControl(World world, Rocket rocket)
            : base(world)
        {
            this.rocket = rocket;
        }

        #region WorldObject

        public override string Identifier
        {
            get { return "RocketControl of " + rocket.Identifier; }
        }

        #endregion WorldObject

        #region IRocketControl

        public double Throttle
        {
            get
            {
                return rocket.raw == FlightGlobals.ActiveVessel ? FlightInputHandler.state.mainThrottle : rocket.raw.ctrlState.mainThrottle;
            }
            set
            {
                value = clamp(value, 0, 1);

                if (value > 0)
                {
                    world.PreventTimeWarping();
                    checkStaging();
                }

                rocket.raw.ctrlState.mainThrottle = (float)value;

                if (rocket.raw == FlightGlobals.ActiveVessel)
                    FlightInputHandler.state.mainThrottle = (float)value;
            }
        }

        public void LockHeading(IEulerAngles3 newHeading)
        {
            lockHeading(Quaternion.Euler(newHeading.X, newHeading.Y, newHeading.Z));
        }

        /// <summary>
        /// Wish for a target heading, pitch and roll. We see what we can do.
        /// </summary>
        /// <param name="pitchAboveHorizon">-90° (down) to 90° up</param>
        /// <param name="degreesFromNorth">0 to 360° (North = 0°, East = 90°, South = 180°, West = 270°)</param>
        /// <param name="roll">--180° to 180°</param>
        public void SetCompassSteering(double pitchAboveHorizon, double degreesFromNorth, double roll)
        {
            Quaternion targetDir = Quaternion.LookRotation(getNorth(), rocket.raw.upAxis);
            targetDir *= Quaternion.Euler(new Vector3((float)-pitchAboveHorizon, (float)degreesFromNorth, (float)roll));
            lockHeading(targetDir);
        }

        public void SetSteering(IVectorXYZ forward)
        {
            lockHeading(Quaternion.LookRotation(v3d(forward), facing * Vector3.up));
        }

        public bool Turned
        {
            get { return TurnDeviation < 0.03; } // less than 0.03° deviation }
        }

        public double TurnDeviation
        {
            get
            {
                return derivation(lockedHeading, this.rocket.raw.Autopilot.SAS.currentRotation);
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

        public IVectorXYZ Up
        {
            get { return vXYZ(rocket.raw.upAxis); }
        }

        public IVectorXYZ OrbitRetrograde
        {
            get { return vXYZ(rocket.raw.obt_velocity.normalized * -1); }
        }

        public IVectorXYZ SurfaceRetrograde
        {
            get { return vXYZ(rocket.raw.srf_velocity.normalized * -1); }
        }

        public double MaxAcceleration
        {
            get { return this.rocket.raw.GetTotalMass() == 0 ? 0 : this.MaxThrust / this.rocket.Mass; }
        }

        public double CurrentThrust
        {
            get
            {
                double thrust = 0.0;

                foreach (Part part in rocket.raw.parts)
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

                foreach (Part part in rocket.raw.parts)
                    foreach (PartModule partModule in part.Modules)
                        if (partModule.isEnabled && partModule is ModuleEngines)
                            thrust += engineThrust((ModuleEngines)partModule, 1.0f);

                return thrust;
            }
        }

        public void Stage()
        {
            rocket.raw.MakeActive();

            if (!rocket.raw.isActiveVessel)
                throw new Exception("Vessel has to be active to activate next stage.");

            if (Staging.separate_ready)
                Staging.ActivateNextStage();
        }

        #endregion IRocketControl

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

        private void lockHeading(Quaternion newHeading)
        {
            newHeading = newHeading * Quaternion.Euler(90, 0, 0);

            enableStabilityAssist();

            world.LiveDebugLog.Add("TurnDeviation: " + TurnDeviation);
            world.LiveDebugLog.Add("New TurnDeviation: " + derivation(lockedHeading, newHeading));
            world.LiveDebugLog.Add("Is on track: " + isOnTrack(newHeading));

            //if (!isOnTrack(newHeading))
            this.rocket.raw.Autopilot.SAS.LockHeading(newHeading, true);

            if (!Turned)
                world.PreventTimeWarping();
        }

        private void enableStabilityAssist()
        {
            if (!rocket.raw.ActionGroups[KSPActionGroup.SAS])
                rocket.raw.ActionGroups.SetGroup(KSPActionGroup.SAS, true);
            if (!rocket.raw.ActionGroups[KSPActionGroup.RCS])
                rocket.raw.ActionGroups.SetGroup(KSPActionGroup.RCS, true);

            if (this.rocket.raw.Autopilot.Mode != VesselAutopilot.AutopilotMode.StabilityAssist)
                this.rocket.raw.Autopilot.SetMode(VesselAutopilot.AutopilotMode.StabilityAssist);
        }


        private double derivation(Quaternion q1, Quaternion q2)
        {
            return Quaternion.Angle(q1, q2);
        }

        private bool isOnTrack(Quaternion newHeading)
        {
            double newTurnDerivation = derivation(lockedHeading, newHeading);
            return newTurnDerivation < 0.01; //|| newTurnDerivation < 0.5 * TurnDeviation;
        }

        private Quaternion facing
        {
            get { return Quaternion.Inverse(Quaternion.Euler(90, 0, 0) * Quaternion.Inverse(rocket.raw.ReferenceTransform.rotation) * Quaternion.identity); }
        }

        private Quaternion rotationVesselSurface
        {
            get
            {
                Quaternion rotationSurface = Quaternion.LookRotation((Vector3)getNorth(), (Vector3)getUp());
                return Quaternion.Inverse(Quaternion.Euler(90, 0, 0) * Quaternion.Inverse(rocket.raw.GetTransform().rotation) * rotationSurface);
            }
        }

        private void checkStaging()
        {
            if (MaxThrust > 0)
                return;

            Stage();
        }

        private Vector3d getNorth()
        {
            return Vector3d.Exclude(getUp(), (rocket.raw.mainBody.position + toDouble(rocket.raw.mainBody.transform.up) * rocket.raw.mainBody.Radius) - rocket.raw.findWorldCenterOfMass()).normalized;
        }

        private Vector3d toDouble(Vector3 value)
        {
            return new Vector3d(value.x, value.y, value.z);
        }

        private Vector3d getUp()
        {
            return (rocket.raw.findWorldCenterOfMass() - rocket.raw.mainBody.position).normalized;
        }

        private Quaternion lockedHeading
        {
            get { return this.rocket.raw.Autopilot.SAS.lockedHeading; }
        }
    }
}
