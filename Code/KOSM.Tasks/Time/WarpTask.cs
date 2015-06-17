using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;
using KOSM.Common;

namespace KOSM.Tasks
{
    public class WarpTask : Task
    {
        protected double secondsToWait = 0;
        protected double timeToWaitFor = -1;

        public WarpTask(IWorld world, int secondsToWait)
        {
            this.secondsToWait = secondsToWait;
        }

        public override void Execute(IWorld world, Mission mission)
        {
            if (timeToWaitFor == -1)
                timeToWaitFor = world.PointInTime + secondsToWait;

            if (!world.WarpTimeTo(timeToWaitFor))
                mission.Complete(world, this);
        }

        public override string Description
        {
            get { return "Warping for " + Format.KerbalTimespan(secondsToWait) + "."; }
        }
    }
}
