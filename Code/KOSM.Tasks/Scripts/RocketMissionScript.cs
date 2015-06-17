using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;

namespace KOSM.Tasks
{
    public abstract class RocketMissionScript : IScript
    {
        protected Mission mission = null;

        public void Update(IWorld world)
        {
            world.LiveDebugLog.Clear();

            // we only consider the first rocket.
            IRocket rocket = world.ActiveRocket;

            // first start -> create mission
            if (mission == null)
                mission = newMission(world, rocket);

            mission.Execute(world);
        }

        protected abstract Mission newMission(IWorld world, IRocket rocket);
    }
}
