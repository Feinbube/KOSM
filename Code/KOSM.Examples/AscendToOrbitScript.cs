using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;
using KOSM.Tasks;

namespace KOSM.Examples
{
    public class AscendToOrbitScript : RocketMissionScript
    {
        protected override Mission newMission(IWorld world, IRocket rocket)
        {
            return new Mission(
                new RaiseToLowOrbitTask(world, rocket),
                new StabilizeOrbitTask(world, rocket)
            );
        }
    }
}
