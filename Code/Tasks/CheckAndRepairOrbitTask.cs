using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Game;
using KOSM.Reporting;

namespace KOSM.Tasks
{
    public class CheckAndRepairOrbitTask : RocketTask
    {
        private double targetHeight;

        public CheckAndRepairOrbitTask(World world, Rocket rocket, double targetHeight)
            : base(world, rocket)
        {
            this.targetHeight = targetHeight;
        }

        public override void Execute(World world, Mission mission)
        {
            throw new NotImplementedException();
        }

        public override string Description
        {
            get { return "Checking orbit. (Height should be " + Format.Distance(targetHeight) + ".)"; }
        }
    }
}
