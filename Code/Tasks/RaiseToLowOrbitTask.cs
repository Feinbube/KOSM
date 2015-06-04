using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Game;
using KOSM.Reporting;

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
                mission.Complete(world, this);
                return;
            }

            rocket.Throttle = (rocket.MainBody.SafeLowOrbit - rocket.Orbit.ApoapsisAltitude) / (rocket.MainBody.SafeLowOrbit * 0.01);
            rocket.SetCompassSteering(90 * Math.Pow(1 - rocket.Altitude / rocket.MainBody.SafeLowOrbit, 4), 90, 0);
        }

        private bool RocketIsHeighEnough()
        {
            return (!rocket.MainBody.HasAtmosphere && rocket.Orbit.ApoapsisAltitude > rocket.MainBody.SafeLowOrbit)
                || (rocket.MainBody.HasAtmosphere && rocket.Altitude > rocket.MainBody.SafeLowOrbit * 0.91);
        }

        public override string Description
        {
            get { return "Raising apoapsis to low orbit. (" + Format.Distance(rocket.MainBody.SafeLowOrbit) + ")"; }
        }
    }
}
