using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Game;
using KOSM.Reporting;

namespace KOSM.Tasks
{
    public class ExecuteManeuverTask : RocketTask
    {
        public ExecuteManeuverTask(World world, Rocket rocket)
            : base(world, rocket)
        {
        }

        Maneuver maneuver = null;
        public override void Execute(World world, Mission mission)
        {
            if (maneuver == null)
                maneuver = rocket.NextManeuver;
            
            if (!TurnAndWait(world, maneuver, maneuver.BurnVector))
                return;

            Details = "Burning for a DeltaV of " + Format.Speed(maneuver.InitialMagnitude) + ".";

            if(!maneuver.Complete)
            {
                rocket.Throttle = Math.Min(maneuver.BurnDuration, 1);
                return;
            }

            Details = "Remaining DeltaV: " + Format.Speed(maneuver.Magnitude) + ". Orbit after maneuver: " + rocket.Orbit.ToString() + ".";
            
            rocket.Throttle = 0;
            maneuver.Remove();
            mission.Complete(world, this);
        }

        public override string Description
        {
            get { return "Executing next maneuver."; }
        }
    }
}
