using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;
using KOSM.Common;

namespace KOSM.Tasks
{
    public class RaiseToLowOrbitTask : RocketTask
    {
        public RaiseToLowOrbitTask(IWorld world, IRocket rocket) : base(world, rocket) { }

        public override void Execute(IWorld world, Mission mission)
        {
            if (RocketIsHeighEnough())
            {   // all done
                rocket.Throttle = 0;
                mission.Complete(world, this);
                return;
            }

            rocket.Throttle = (rocket.Body.SafeLowOrbitAltitude - rocket.Orbit.Apoapsis.Altitude) / (rocket.Body.SafeLowOrbitAltitude * 0.01);
            rocket.SetCompassSteering(90 * Math.Pow(1 - rocket.Altitude / rocket.Body.SafeLowOrbitAltitude, 4), 90, 0);
        }

        private bool RocketIsHeighEnough()
        {
            return (!rocket.Body.HasAtmosphere && rocket.Orbit.Apoapsis.Altitude > rocket.Body.SafeLowOrbitAltitude)
                || (rocket.Body.HasAtmosphere && rocket.Altitude > Math.Min(rocket.Body.SafeLowOrbitAltitude * 0.91, rocket.Body.AtmosphereHeight));
        }

        public override string Description
        {
            get { return "Raising apoapsis to low orbit. (" + Format.Distance(rocket.Body.SafeLowOrbitAltitude) + ")"; }
        }
    }
}
