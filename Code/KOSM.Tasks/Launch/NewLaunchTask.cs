using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;

namespace KOSM.Tasks
{
    public class NewLaunchTask : RocketTask
    {
        string rocketDesign = "";

        public NewLaunchTask(IWorld world, IRocket rocket, string rocketDesign)
            : base(world, rocket)
        {
            this.rocketDesign = rocketDesign;
        }

        public override void Execute(IWorld world, Mission mission)
        {
            world.Launch(rocketDesign);
        }

        public override string Description
        {
            get { return "Launch a new rocket of type " +  rocketDesign + "."; }
        }
    }
}
