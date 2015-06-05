using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Game;
using KOSM.States;

namespace KOSM.Tasks
{
    public class LandAtTask : RocketTask
    {
        protected OnGroundState objective = null;
        public LandAtTask(World world, Rocket rocket, OnGroundState objective)
            : base(world, rocket)
        {
            this.objective = objective;
        }

        public override void Execute(World world, Mission mission)
        {
            mission.PushAfter(this,
                    new DeOrbitTask(world, rocket, objective),
                    new DescentToHoverTask(world, rocket, objective),
                    new FinalDescentTask(world, rocket, objective)
                    //new NewDescent(world, rocket, objective)
                    );
            mission.Complete(world, this);
        }

        public override string Description
        {
            get { return "Landing at " + objective.ToString() + "."; }
        }
    }
}
