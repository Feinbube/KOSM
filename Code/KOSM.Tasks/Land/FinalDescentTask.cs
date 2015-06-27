using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;

namespace KOSM.Tasks
{
    public class FinalDescentTask : RocketTask
    {
        protected IOnGroundState destination = null;
        public FinalDescentTask(IWorld world, IRocket rocket, IOnGroundState destination)
            : base(world, rocket)
        {
            this.destination = destination;
        }

        public override void Execute(IWorld world, Mission mission)
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
            get { return "Decending to slowly to " + destination + "."; }
        }
    }
}
