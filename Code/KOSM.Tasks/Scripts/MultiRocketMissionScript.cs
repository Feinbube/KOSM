using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;

namespace KOSM.Tasks
{
    public abstract class MultiRocketMissionScript : IScript
    {
        protected Dictionary<string, Mission> missions = new Dictionary<string, Mission>();

        public virtual void Update(IWorld world)
        {
            world.LiveDebugLog.Clear();
            world.LiveDebugLog.Add("Number or rockets: " + world.Rockets.Count);
            for (int i = 0; i < world.Rockets.Count; i++)
            {
                IRocket rocket = world.Rockets[i];

                world.LiveDebugLog.Add("### Controlling Vessel #" + i + " (" + rocket.Name + ") ######");

                // first start -> create mission
                if (!missions.ContainsKey(rocket.Identifier))
                    missions.Add(rocket.Identifier, newMission(world, rocket));

                missions[rocket.Identifier].Execute(world);
            }
        }

        protected abstract Mission newMission(IWorld world, IRocket rocket);
    }
}
