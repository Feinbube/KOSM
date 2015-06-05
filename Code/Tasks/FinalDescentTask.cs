using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Game;
using KOSM.States;

namespace KOSM.Tasks
{
    public class FinalDescentTask : RocketTask
    {
        protected OnGroundState objective = null;
        public FinalDescentTask(World world, Rocket rocket, OnGroundState objective)
            : base(world, rocket)
        {
            this.objective = objective;
        }

        public override void Execute(World world, Mission mission)
        {
            double maxVerticalSpeed = 0.2;

            if (rocket.Landed || rocket.AltitudeOverGround < maxVerticalSpeed / 2)
            {
                rocket.Throttle = 0;
                Details = "Touch down!";
                mission.Complete(world, this);
                return;
            }

            rocket.LowerGear();

            rocket.SetSteering(rocket.Up);
            rocket.Throttle = 0.5 * Math.Max(0, Math.Min(1, -rocket.VerticalSpeed - maxVerticalSpeed));
        }

        public override string Description
        {
            get { return "Decending to slowly to " + objective.ToString() + "."; }
        }
    }
}
