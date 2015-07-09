using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;
using KOSM.Common;

namespace KOSM.Tasks
{
    public class WarpToEncounter : Task
    {
        protected IRocket rocket = null;
        protected IBody body = null;

        public WarpToEncounter(IWorld world, IRocket rocket, IBody body)
        {
            this.rocket = rocket;
            this.body = body;
        }

        public override void Execute(IWorld world, Mission mission)
        {
            if (this.rocket.Body == body)
            {
                world.PreventTimeWarping();
                mission.Complete(world, this);
                return;
            }
            
            world.WarpTimeTo(world.PointInTime + rocket.Orbit.Period);
        }

        public override string Description
        {
            get { return "Warping to next encounter."; }
        }
    }
}
