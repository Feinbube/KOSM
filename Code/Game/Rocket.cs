using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                world.DebugLog.Add("Setting throttle to " + value);

                vessel.ctrlState.mainThrottle = (float)clamp(value, 0, 1);

                if (vessel == FlightGlobals.ActiveVessel)
                    FlightInputHandler.state.mainThrottle = (float)clamp(value, 0, 1);
            }
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

        public void SetCompassSteering(double degreesFromNorth, double pitchAboveHorizon)
        {
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
