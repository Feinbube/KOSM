using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;

namespace KOSM.Tasks
{
    public abstract class RocketTask : Task
    {
        protected IRocket rocket = null;

        public RocketTask(IWorld world, IRocket rocket)
        {
            this.rocket = rocket;
        }

        protected bool TurnAndWait(IWorld world, IManeuver maneuver, IVectorXYZ vector)
        {
            Details = "Warping to next maneuver.";
            if (world.OneTickWarpTimeTo(maneuver.TimeOfTurn))
                return false;

            Details = "Turning ship for maneuver.";
            rocket.SetSteering(vector);

            if (rocket.Turned)
                if (world.OneTickWarpTimeTo(maneuver.TimeOfBurn - 10))
                    return false;

            return maneuver.TimeTillBurn <= 0;
        }

        protected bool TurnAndWait(IWorld world, double timeToManeuver, IVectorXYZ vector)
        {
            Details = "Warping to next maneuver.";
            if (world.OneTickWarpTimeTo(timeToManeuver - 180))
                return false;

            Details = "Turning ship for maneuver.";
            rocket.SetSteering(vector);

            if (rocket.Turned)
                if (world.OneTickWarpTimeTo(timeToManeuver - 10))
                    return false;

            return timeToManeuver <= 0;
        }
    }
}
