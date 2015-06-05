using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Game;
using KOSM.Reporting;

namespace KOSM.Tasks
{
    public abstract class RocketTask : Task
    {
        protected Rocket rocket = null;

        public RocketTask(World world, Rocket rocket)
        {
            this.rocket = rocket;
        }

        protected bool TurnAndWait(World world, Maneuver maneuver, Vector3d vector)
        {
            Details = "Waiting for next maneuver.";
            if (world.WarpTimeTo(maneuver.TimeOfTurn))
                return false;

            Details = "Turning ship for maneuver.";
            rocket.SetSteering(vector);

            if (rocket.Turned)
                if (world.WarpTimeTo(maneuver.TimeOfBurn))
                    return false;

            return maneuver.TimeTillBurn <= 0;
        }

        protected bool TurnAndWait(World world, double timeToManeuver, Vector3d vector)
        {
            Details = "Waiting for next maneuver.";
            if (world.WarpTime(timeToManeuver - 60))
                return false;

            Details = "Turning ship for maneuver.";
            rocket.SetSteering(vector);

            if (rocket.Turned)
                if (world.WarpTime(timeToManeuver))
                    return false;

            return timeToManeuver <= 0;
        }
    }
}
