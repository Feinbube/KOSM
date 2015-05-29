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

        private Quaternion rotationVesselSurface // TODO: understand and replace with something simpler (less math, more using whatever the game provides)
        {
            get
            {
                Vector3d centerOfMass = vessel.findWorldCenterOfMass();
                Vector3d up = (centerOfMass - vessel.mainBody.position).normalized;

                Vector3d north = Vector3d.Exclude(up, (vessel.mainBody.position + vessel.mainBody.transform.up * (float)vessel.mainBody.Radius) - centerOfMass).normalized;
                Vector3d east = vessel.mainBody.getRFrmVel(centerOfMass).normalized;
                Vector3d forward = vessel.GetTransform().up;
                Quaternion rotationSurface = Quaternion.LookRotation((Vector3)north, (Vector3)up);
                Quaternion rotationVesselSurface = Quaternion.Inverse(Quaternion.Euler(90, 0, 0) * Quaternion.Inverse(vessel.GetTransform().rotation) * rotationSurface);
                return rotationVesselSurface;
            }
        }

        /// <summary>
        /// 0 to 360° (North = 0°, East = 90°, South = 180°, West = 270°)
        /// </summary>
        public double Heading { get { return rotationVesselSurface.eulerAngles.y; } }

        /// <summary>
        /// -90° (down) to 90° up
        /// </summary>
        public double Pitch { get { return (rotationVesselSurface.eulerAngles.x > 180) ? (360.0 - rotationVesselSurface.eulerAngles.x) : -rotationVesselSurface.eulerAngles.x; } }

        /// <summary>
        /// -180° to 180°
        /// </summary>
        public double Roll { get { return (rotationVesselSurface.eulerAngles.z > 180) ? (rotationVesselSurface.eulerAngles.z - 360.0) : rotationVesselSurface.eulerAngles.z; } }

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
        /// <param name="targetHeading">0 to 360° (North = 0°, East = 90°, South = 180°, West = 270°)</param>
        /// <param name="targetPitch">-90° (down) to 90° up</param>
        /// <param name="targetRoll">--180° to 180°</param>
        public void SetCompassSteering(double targetHeading, double targetPitch, double targetRoll)
        {
            world.DebugLog.Add("Setting Compass Steering: ");
            world.DebugLog.Add("  Heading from " + Heading + " to " + targetHeading);
            world.DebugLog.Add("  Pitch from " + Pitch + " to " + targetPitch);
            world.DebugLog.Add("  Roll from " + Roll + " to " + targetRoll);
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
