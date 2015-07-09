using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;
using KOSM.Tasks;
using KOSM.Common;

namespace KOSM.Examples
{
    public class TestScript : MultiRocketMissionScript
    {
        protected override Mission newMission(IWorld world, IRocket rocket)
        {
            return new Mission(
                    new WarpToEncounter(world, rocket, world.FindBodyByName("Duna")),
                    new LandAtTask(world, rocket, new OnGroundState(world, "Duna"))
                );
        }
    }
}
