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

        public double Altitude { get { return vessel.altitude; } }
        public double AltitudeOverGround { get { return vessel.terrainAltitude; } }
        public double Throttle
        {
            get { return vessel == FlightGlobals.ActiveVessel ? FlightInputHandler.state.mainThrottle : vessel.ctrlState.mainThrottle; }
            set
            {
                value = clamp(value, 0, 1);

                world.DebugLog.Add("Setting throttle to " + value);
                vessel.ctrlState.mainThrottle = (float)value;

                if (vessel == FlightGlobals.ActiveVessel)
                    FlightInputHandler.state.mainThrottle = (float)value;
            }
        }

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

        public Body MainBody { get { return world.FindBodyByName(vessel.mainBody.GetName()); } }

        public Orbit Orbit
        {
            get
            {
                if (orbit == null)
                    orbit = new Orbit(world, vessel.orbit);
                return orbit;
            }
        }

        public Rocket(World world, Vessel vessel)
            : base(world)
        {
            this.vessel = vessel;
        }

        /// <summary>
        /// Wish for a target heading, pitch and roll. We see what we can do.
        /// </summary>
        /// <param name="pitchAboveHorizon">-90° (down) to 90° up</param>
        /// <param name="degreesFromNorth">0 to 360° (North = 0°, East = 90°, South = 180°, West = 270°)</param>
        /// <param name="roll">--180° to 180°</param>
        public void SetCompassSteering(double pitchAboveHorizon, double degreesFromNorth, double roll)
        {
            world.DebugLog.Add("Setting heading to " + pitchAboveHorizon + "°, " + degreesFromNorth + "°, " + roll + "°");

            var targetDir = Quaternion.LookRotation(getNorth(), vessel.upAxis);
            targetDir *= Quaternion.Euler(new Vector3((float)-pitchAboveHorizon, (float)degreesFromNorth, (float)roll));
            targetDir = targetDir * Quaternion.Euler(90, 0, 0);
            LockHeading(targetDir);
        }

        public void LockHeading(Quaternion newHeading)
        {
            this.vessel.Autopilot.SetMode(VesselAutopilot.AutopilotMode.StabilityAssist);
            this.vessel.Autopilot.SAS.LockHeading(newHeading, true);
        }

        private Vector3d getNorth()
        {
            return Vector3d.Exclude(getUp(), (vessel.mainBody.position + toDouble(vessel.mainBody.transform.up) * vessel.mainBody.Radius) - vessel.findWorldCenterOfMass()).normalized;
        }

        private Vector3d getUp()
        {
            return (vessel.findWorldCenterOfMass() - vessel.mainBody.position).normalized;
        }

        private Vector3d getEast()
        {
            return vessel.mainBody.getRFrmVel(vessel.findWorldCenterOfMass()).normalized;
        }

        private Vector3d getForward()
        {
            return vessel.GetTransform().up;
        }

        private Vector3d toDouble(Vector3 value)
        {
            return new Vector3d(value.x, value.y, value.z);
        }

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