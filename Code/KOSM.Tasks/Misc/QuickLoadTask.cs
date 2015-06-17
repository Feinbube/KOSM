using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;

namespace KOSM.Tasks
{
    public class QuickLoadTask : RocketTask
    {
        public QuickLoadTask(IWorld world, IRocket rocket)
            : base(world, rocket)
        {
        }

        public override void Execute(IWorld world, Mission mission)
        {
            world.QuickLoad();
        }

        public override string Description
        {
            get { return "Quickloading."; }
        }
    }
}
