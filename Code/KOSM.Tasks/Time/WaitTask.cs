using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;
using KOSM.Common;

namespace KOSM.Tasks
{
    public class WaitTask : Task
    {
        protected double secondsToWait = 0;
        protected double timeToWaitFor = -1;

        public WaitTask(IWorld world, double secondsToWait)
        {
            this.secondsToWait = secondsToWait;
        }

        public override void Execute(IWorld world, Mission mission)
        {
            if (timeToWaitFor == -1)
                timeToWaitFor = world.PointInTime + secondsToWait;

            if (timeToWaitFor <= world.PointInTime)
                mission.Complete(world, this);
        }

        public override string Description
        {
            get { return "Waiting for " + Format.KerbalTimespan(secondsToWait) + "."; }
        }
    }
}
