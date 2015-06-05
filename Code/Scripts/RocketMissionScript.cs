using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Game;
using KOSM.Tasks;

namespace KOSM.Scripts
{
    public abstract class RocketMissionScript : IScript
    {
        protected Mission mission = null;

        public void Update(World world)
        {
            world.LiveDebugLog.Clear();

            // we only consider the first rocket.
            Rocket rocket = world.Rockets[0];

            // first start -> create mission
            if (mission == null)
                mission = newMission(world, rocket);

            mission.Execute(world);
        }

        public void Reset(World world)
        {
            mission = null;
            world.MissionLog.Clear();
        }

        protected abstract Mission newMission(World world, Rocket rocket);
    }
}
