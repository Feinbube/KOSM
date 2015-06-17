using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;

namespace KOSM.Tasks
{
    public class LandAtTask : RocketTask
    {
        protected IOnGroundState objective = null;
        public LandAtTask(IWorld world, IRocket rocket, IOnGroundState objective)
            : base(world, rocket)
        {
            this.objective = objective;
        }

        public override void Execute(IWorld world, Mission mission)
        {
            mission.PushAfter(this,
                    new DeOrbitTask(world, rocket, objective),
                    new DescentToHoverTask(world, rocket, objective),
                    new FinalDescentTask(world, rocket, objective)
                    );
            mission.Complete(world, this);
        }

        public override string Description
        {
            get { return "Landing at " + objective.ToString() + "."; }
        }
    }
}
