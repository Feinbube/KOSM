using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;
using KOSM.Common;

namespace KOSM.Tasks
{
    public class WarpTill : RocketTask
    {
        private Func<IWorld, IRocket, bool> terminateCondition;
        private Func<IWorld, IRocket, double> warpFactor;
        private string description = "";

        public WarpTill(IWorld world, IRocket rocket, Func<IWorld, IRocket, bool> terminateCondition, Func<IWorld, IRocket, double> warpFactor, string description)
            : base(world, rocket)
        {
            this.terminateCondition = terminateCondition;
            this.warpFactor = warpFactor;
            this.description = description;
        }

        public override void Execute(IWorld world, Mission mission)
        {
            if (terminateCondition(world, rocket))
            {
                world.PreventTimeWarping();
                mission.Complete(world, this);
                return;
            }

            world.OneTickWarpTimeBy(warpFactor(world, rocket));
        }

        public override string Description
        {
            get { return description; }
        }
    }
}
