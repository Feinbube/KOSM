using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;

namespace KOSM.Tasks
{
    public class LandAtTask : RocketTask
    {
        protected IOnGroundState destination = null;
        public LandAtTask(IWorld world, IRocket rocket, IOnGroundState destination)
            : base(world, rocket)
        {
            this.destination = destination;
        }

        public override void Execute(IWorld world, Mission mission)
        {
            mission.PushAfter(this,
                    new DeOrbitTask(world, rocket, destination),
                    new DescentToHoverTask(world, rocket, destination),
                    new FinalDescentTask(world, rocket, destination)
                    );
            mission.Complete(world, this);
        }

        public override string Description
        {
            get { return "Landing at " + destination + "."; }
        }
    }
}
