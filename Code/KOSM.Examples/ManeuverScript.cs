using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;
using KOSM.Tasks;

namespace KOSM.Examples
{
    public class ManeuverScript : RocketMissionScript
    {
        protected override Mission newMission(IWorld world, IRocket rocket)
        {
            return new Mission(
                new ExecuteManeuverTask(world, rocket)
            );
        }
    }
}
