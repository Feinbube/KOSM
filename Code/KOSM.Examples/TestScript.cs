using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;
using KOSM.Tasks;

namespace KOSM.Examples
{
    public class TestScript : MultiRocketMissionScript
    {
        protected override Mission newMission(IWorld world, IRocket rocket)
        {
            return new Mission(new TestTask(world, rocket));
        }
    }
}
