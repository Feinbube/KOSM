using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;
using KOSM.Common;
using KOSM.Tasks;

namespace KOSM.Examples
{
    public class LandScript : RocketMissionScript
    {
        protected override Mission newMission(IWorld world, IRocket rocket)
        {
            return new Mission(
                new LandAtTask(world, rocket, new OnGroundState(world, rocket.Body.Name))
            );
        }
    }
}
