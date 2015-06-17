using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;
using KOSM.Common;

namespace KOSM.Tasks
{
    public class WaitForRealTimeTask : Task
    {
        protected double secondsToWait = 0;
        protected DateTime timeToWaitFor = DateTime.MinValue;

        public WaitForRealTimeTask(IWorld world, int secondsToWait)
        {
            this.secondsToWait = secondsToWait;
        }

        public override void Execute(IWorld world, Mission mission)
        {
            if (timeToWaitFor == DateTime.MinValue)
                timeToWaitFor = DateTime.Now.AddSeconds(secondsToWait);

            if (timeToWaitFor <= DateTime.Now)
                mission.Complete(world, this);
        }

        public override string Description
        {
            get { return "Waiting for " + Format.KerbalTimespan(secondsToWait) + " (wall clock time)."; }
        }
    }
}
