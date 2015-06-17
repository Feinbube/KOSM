using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;
using KOSM.Common;

namespace KOSM.Tasks
{
    public class ExecuteManeuverTask : RocketTask
    {
        public ExecuteManeuverTask(IWorld world, IRocket rocket)
            : base(world, rocket)
        {
        }

        IManeuver maneuver = null;
        public override void Execute(IWorld world, Mission mission)
        {
            if (maneuver == null)
                maneuver = rocket.NextManeuver;
            
            if (!TurnAndWait(world, maneuver, maneuver.BurnVector))
                return;

            Details = "Burning for a DeltaV of " + Format.Speed(maneuver.DeltaV.Magnitude) + ".";

            if(!maneuver.Completed)
            {
                rocket.Throttle = Math.Min(maneuver.BurnDuration, 1);
                return;
            }

            Details = "Remaining DeltaV: " + Format.Speed(maneuver.BurnVector.Magnitude) + ". Orbit after maneuver: " + rocket.Orbit.ToString() + ".";
            
            rocket.Throttle = 0;
            maneuver.Complete();
            mission.Complete(world, this);
        }

        public override string Description
        {
            get { return "Executing next maneuver."; }
        }
    }
}
