using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Game;
using KOSM.Scripts;
using KOSM.Tasks;
using KOSM.States;

namespace KOSM.Examples
{
    public class PresentationScript : MultiRocketMissionScript
    {
        protected override Mission newMission(World world, Rocket rocket)
        {
            return new Mission(
                new MoveFromToTask(world, rocket, new OnGroundState(world, "Kerbin"), new OnGroundState(world, "Mun")),
                new MoveFromToTask(world, rocket, new OnGroundState(world, "Mun"), new OnGroundState(world, "Minmus")),
                new MoveFromToTask(world, rocket, new OnGroundState(world, "Minmus"), new OnGroundState(world, "Kerbin"))
            );
        }
    }
}
