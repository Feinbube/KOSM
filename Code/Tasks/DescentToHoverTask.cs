using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Game;
using KOSM.States;

namespace KOSM.Tasks
{
    public class DescentToHoverTask : RocketTask
    {
        protected OnGroundState objective = null;
        public DescentToHoverTask(World world, Rocket rocket, OnGroundState objective)
            : base(world, rocket)
        {
            this.objective = objective;
        }

        public override void Execute(World world, Mission mission)
        {
            double hoverHeight = 3;
            double maxVerticalSpeed = 2;

            if (-rocket.VerticalSpeed < maxVerticalSpeed && rocket.AltitudeOverGround <= 2 * hoverHeight)
            {
                mission.Complete(world, this);
                return;
            }

            if (rocket.AltitudeOverGround > 2 * rocket.MainBody.GravityAtSealevel + hoverHeight)
            {
                rocket.SetSteering(rocket.SurfaceRetrograde);
                rocket.Throttle = coarseThrottle(world, rocket, hoverHeight);
            }
            else
            {
                rocket.SetSteering(rocket.Up);
                rocket.Throttle = fineThrottle(world, rocket, hoverHeight);
            }
        }

        private double fineThrottle(World world, Rocket rocket, double hoverHeight)
        {
            double breakingAcceleration = Math.Pow(rocket.SurfaceVelocityMagnitude, 2) / (rocket.AltitudeOverGround - hoverHeight);
            return breakingAcceleration / (rocket.MaxAcceleration - rocket.MainBody.GravityAtSealevel);
        }

        private double coarseThrottle(World world, Rocket rocket, double hoverHeight)
        {
            double fine = fineThrottle(world, rocket, hoverHeight);
            return fine > 0.8 ? fine : 0;
        }

        public override string Description
        {
            get { return "Decending to " + objective.ToString() + "."; }
        }
    }
}
