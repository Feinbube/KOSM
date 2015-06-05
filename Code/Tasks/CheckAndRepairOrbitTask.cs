using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Game;
using KOSM.Reporting;

namespace KOSM.Tasks
{
    public class CheckAndRepairOrbitTask : RocketTask
    {
        private double targetAltitude;

        public CheckAndRepairOrbitTask(World world, Rocket rocket, double targetAltitude)
            : base(world, rocket)
        {
            this.targetAltitude = targetAltitude;
        }

        public override void Execute(World world, Mission mission)
        {
            if (rocket.Orbit.PeriapsisAltitude > targetAltitude * 0.95)
            {
                mission.Complete(world, this);
                return;
            }

            this.Details = "Resulting orbit is too low! Trying my best to get back on track.";

            if ((rocket.Orbit.MovingTowardsPeriapsis && rocket.Altitude < targetAltitude) || (rocket.Orbit.MovingTowardsApoapsis && rocket.Orbit.ApoapsisAltitude < targetAltitude))
            {
                rocket.SetSteering(rocket.Up);
                rocket.Throttle = 1;
                return;
            }

            rocket.Throttle = 0;

            this.Details = "Raising periapsis from " + Format.Distance(rocket.Orbit.PeriapsisAltitude) + " to " + Format.Distance(targetAltitude);

            rocket.AddApoapsisManeuver(targetAltitude);
            mission.PushBefore(this, new ExecuteManeuverTask(world, rocket));
        }

        public override string Description
        {
            get { return "Checking orbit. (Height should be " + Format.Distance(targetAltitude) + ".)"; }
        }
    }
}
