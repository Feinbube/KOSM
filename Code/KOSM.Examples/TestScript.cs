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
            var body = world.FindBodyByName("Duna");
            return new Mission(
                    new AdjustEncounterTask(world, rocket),
                    new WarpTill(world, rocket, (w, r) => r.Body == body, (w, r) => body.Position.Minus(r.Position).Magnitude, "Warping to encounter with Duna."),
                    new WarpTask(world, 2, true), // wait some time to ensure the game understands that we are in the encounter now
                    new LandAtTask(world, rocket, new OnGroundState(world, "Duna"))
                );
        }
    }
}
