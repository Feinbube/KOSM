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
            throw new NotImplementedException();
        }

        public override string InfoText
        {
            get { return "Landing at " + objective + "."; }
        }
    }
}
