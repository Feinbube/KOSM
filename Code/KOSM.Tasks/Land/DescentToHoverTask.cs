using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;

namespace KOSM.Tasks
{
    public class DescentToHoverTask : RocketTask
    {
        protected IOnGroundState objective = null;
        public DescentToHoverTask(IWorld world, IRocket rocket, IOnGroundState objective)
            : base(world, rocket)
        {
            this.objective = objective;
        }

        public override void Execute(IWorld world, Mission mission)
        {
            double hoverHeight = 3;
            double maxVerticalSpeed = 2;

            if (rocket.Landed || (-rocket.VerticalSpeed < maxVerticalSpeed && rocket.AltitudeOverGround <= 2 * hoverHeight))
            {
                mission.Complete(world, this);
                return;
            }

            if (rocket.MaxAcceleration == 0)
                rocket.Stage();

            if (rocket.AltitudeOverGround > 2 * rocket.Body.GravityAtSealevel + hoverHeight)
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

        private double fineThrottle(IWorld world, IRocket rocket, double hoverHeight)
        {
            double breakingAcceleration = Math.Pow(rocket.SurfaceVelocity.Magnitude, 2) / (rocket.AltitudeOverGround - hoverHeight);
            return breakingAcceleration / (rocket.MaxAcceleration - rocket.Body.GravityAtSealevel);
        }

        private double coarseThrottle(IWorld world, IRocket rocket, double hoverHeight)
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
