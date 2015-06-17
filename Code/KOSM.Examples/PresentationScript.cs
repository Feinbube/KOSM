using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;
using KOSM.Common;
using KOSM.Tasks;

namespace KOSM.Examples
{
    public class PresentationScript : MultiRocketMissionScript
    {
        Random random = new Random();

        int maxRuntime = 3600;

        DateTime start = DateTime.Now;
        bool first = true;

        public override void Update(IWorld world)
        {
            base.Update(world);

            if (first && world.Rockets.Count > 0)
            {
                start = DateTime.Now;
                first = false;
            }

            if (!first && (world.Rockets.Count == 0 || start.AddSeconds(maxRuntime) < DateTime.Now))
                world.QuickLoad();
        }

        protected override Mission newMission(IWorld world, IRocket rocket)
        {
            return new Mission(
                new RunMissionTask(world, rocket, new OnGroundState(world, "Kerbin"), new InOrbitState(world, "Kerbin")),
                new WaitForRealTimeTask(world, 10),
                new WarpTask(world, random.Next(10, 1000)),
                new RunMissionTask(world, rocket, new InOrbitState(world, "Kerbin"), new OnGroundState(world, "Kerbin")),
                new WaitForRealTimeTask(world, 10),
                new QuickLoadTask(world, rocket)
            );

            //return new Mission(
            //    new RunMissionTask(world, rocket, new OnGroundState(world, "Kerbin"), new OnGroundState(world, "Mun")),
            //    new RunMissionTask(world, rocket, new OnGroundState(world, "Mun"), new OnGroundState(world, "Minmus")),
            //    new RunMissionTask(world, rocket, new OnGroundState(world, "Minmus"), new OnGroundState(world, "Kerbin"))
            //);
        }
    }
}
