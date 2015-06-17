using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;

namespace KOSM.Tasks
{
    public class InclinationChangeTask : RocketTask
    {
        private IBody targetBody;

        public InclinationChangeTask(IWorld world, IRocket rocket, IBody targetBody)
            : base(world, rocket)
        {
            this.targetBody = targetBody;
        }

        public override void Execute(IWorld world, Mission mission)
        {
            rocket.AddInclinationChangeManeuver(targetBody);

            mission.PushAfter(this, new ExecuteManeuverTask(world, rocket));
            mission.Complete(world, this);
        }

        public override string Description
        {
            get { return String.Format("Changing inclination to {0:0.00}.", targetBody.Orbit.Inclination); }
        }
    }
}
