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
        IManeuver maneuver = null;

        public ExecuteManeuverTask(IWorld world, IRocket rocket)
            : base(world, rocket)
        {
        }

        public ExecuteManeuverTask(IWorld world, IRocket rocket, Func<IWorld, IRocket, bool> completed)
            : base(world, rocket)
        {
        }
        
        public override void Execute(IWorld world, Mission mission)
        {
            if (!rocket.HasManeuver)
            {
                mission.Abort(world, this);
                return;
            }

            if (maneuver == null)
                maneuver = rocket.NextManeuver;

            if(maneuver.Completed)
            {
                rocket.Throttle = 0;
                maneuver.Complete();
                mission.Complete(world, this);

                Details = "Remaining DeltaV: " + Format.Speed(maneuver.BurnVector.Magnitude) + ". Orbit after maneuver: " + rocket.Orbit + ".";
                return;
            }

            if (!TurnAndWait(world, maneuver, maneuver.BurnVector))
                return;

            Details = "Burning for a DeltaV of " + Format.Speed(maneuver.DeltaV.Magnitude) + ".";

            if(!maneuver.Completed)
            {
                rocket.Throttle = Math.Min(maneuver.BurnDuration, 1);
                return;
            }
        }

        public override string Description
        {
            get { return "Executing next maneuver."; }
        }
    }
}
