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
        protected double secondsToWarp = 0;
        protected double timeToWaitFor = -1;
        protected bool persistent;

        public WarpTask(IWorld world, double secondsToWarp, bool persistent)
        {
            this.secondsToWarp = secondsToWarp;
            this.persistent = persistent;
        }

        public override void Execute(IWorld world, Mission mission)
        {
            if (timeToWaitFor == -1)
                timeToWaitFor = world.PointInTime + secondsToWarp;

            if (persistent ? !world.PersistentWarpTimeTo(timeToWaitFor) : !world.OneTickWarpTimeTo(timeToWaitFor))
                mission.Complete(world, this);
        }

        public override string Description
        {
            get { return "Warping for " + Format.KerbalTimespan(secondsToWarp) + "."; }
        }
    }
}
