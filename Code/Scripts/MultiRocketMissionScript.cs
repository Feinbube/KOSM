using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Game;
using KOSM.Tasks;

namespace KOSM.Scripts
{
    public abstract class MultiRocketMissionScript : IScript
    {
        protected List<Mission> missions = new List<Mission>();

        public void Update(World world)
        {
            world.DebugLog.Clear();

            for (int i = 0; i < world.Rockets.Count; i++)
            {
                Rocket rocket = world.Rockets[i];

                world.DebugLog.Add("### Controlling Vessel #" + i + " (" + rocket.Name + ") ######");

                // first start -> create mission
                if (missions.Count <= i || missions[i] == null)
                    missions.Add(newMission(world, rocket));

                missions[i].Execute(world);
            }
        }

        protected abstract Mission newMission(World world, Rocket rocket);
    }
}
