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

            double burnDuration = maneuver.BurnDuration(rocket);
            double timeToManeuver = maneuver.SecondsLeft - burnDuration / 2;
            
            Details = "Waiting for next maneuver at " + Format.MissionTime(rocket.MissionTime + timeToManeuver) + ".";
            if(world.WarpTime(timeToManeuver - 60))
                return;

            Details = "Turning ship for maneuver.";
            rocket.SetSteering(maneuver.BurnVector);

            if(rocket.Turned)
                if(world.WarpTime(timeToManeuver))
                    return;

            if( timeToManeuver > 0)
                return;

            Details = "Burning for a DeltaV of " + Format.Speed(maneuver.InitialMagnitude) + ".";

            if(!maneuver.Complete)
            {
                rocket.Throttle = Math.Min(burnDuration, 1);
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
