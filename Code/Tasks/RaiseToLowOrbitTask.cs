using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Game;

namespace KOSM.Tasks
{
    public class RaiseToLowOrbitTask : RocketTask
    {
        public RaiseToLowOrbitTask(World world, Rocket rocket) : base(world, rocket) { }

        public override void Execute(World world, Mission mission)
        {
            if (RocketIsHeighEnough())
            {   // all done
                rocket.Throttle = 0;
                mission.Complete(this);
                return;
            }

            double safeLowOrbit = rocket.MainBody.LowOrbit * 1.1;
            rocket.Throttle = (safeLowOrbit - rocket.Orbit.ApoapsisAltitude) / (safeLowOrbit * 0.01);
            rocket.SetCompassSteering(90, 90 * Math.Pow(1 - rocket.Altitude / safeLowOrbit, 4), 0);
        }

        private bool RocketIsHeighEnough()
        {
            double safeLowOrbit = rocket.MainBody.LowOrbit * 1.1;
            return (!rocket.MainBody.HasAtmosphere && rocket.Orbit.ApoapsisAltitude > safeLowOrbit)
                || (rocket.MainBody.HasAtmosphere && rocket.Orbit.ApoapsisAltitude > safeLowOrbit && rocket.Altitude > Math.Min(safeLowOrbit, rocket.Orbit.ApoapsisAltitude * 0.9));
        }

        public override string InfoText
        {
            get { return "Raising apoapsis to low orbit."; }
        }
    }
}
