using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;

namespace KOSM.Tasks
{
    public class HohmannTransferTask : RocketTask
    {
        protected IBody targetBody = null;

        public HohmannTransferTask(IWorld world, IRocket rocket, IBody targetBody)
            : base(world, rocket)
        {
            this.targetBody = targetBody;
        }

        public override void Execute(IWorld world, Mission mission)
        {
            if(!rocket.InCircularOrbit)
            {
                mission.PushBefore(this, new CircularizeOrbitTask(world, rocket));
                return;
            }

            if (Math.Abs(rocket.Orbit.Inclination - targetBody.Orbit.Inclination) > 0.1)
            {
                mission.PushBefore(this, new InclinationChangeTask(world, rocket, targetBody));
                return;
            }

            rocket.AddHohmannManeuver(targetBody);
            mission.PushAfter(this, new ExecuteManeuverTask(world, rocket));
            mission.Complete(world, this);
        }

        public override string Description
        {
            get { return "Executing a Hohmann transfer to " + targetBody.Name + "."; }
        }
    }
}
