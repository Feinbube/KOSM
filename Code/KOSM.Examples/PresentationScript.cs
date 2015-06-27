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

            if (world.ActiveRocket != null)
                world.Camera.BodyBehindRocket(world.ActiveRocket.Body, world.ActiveRocket);

            if (first && world.Rockets.Count > 0)
            {
                start = DateTime.Now;
                first = false;
            }

            if (!first)
            {
                if (world.Rockets.Count == 0)
                {
                    world.PersistentDebugLog.Add("No rockets alive. Starting a new launch.");
                    world.Launch(randomRocketDesign(world));
                }
                if (start.AddSeconds(maxRuntime) < DateTime.Now)
                {
                    world.PersistentDebugLog.Add("Running out of time. Starting a new launch.");
                    world.Launch(randomRocketDesign(world));
                }

            }
        }

        protected override Mission newMission(IWorld world, IRocket rocket)
        {
            return newMissionDunaAndBack(world, rocket);
        }

        protected Mission newMissionDresAndBack(IWorld world, IRocket rocket)
        {
            return new Mission(
                new DirectTransferMissionTask(world, rocket, new OnGroundState(world, "Kerbin"), new OnGroundState(world, "Dres")),
                new WaitForRealTimeTask(world, 300),
                new DirectTransferMissionTask(world, rocket, new OnGroundState(world, "Dres"), new OnGroundState(world, "Kerbin")),
                new WaitForRealTimeTask(world, 300),
                new NewLaunchTask(world, rocket, randomRocketDesign(world))
            );
        }

        protected Mission newMissionDunaAndBack(IWorld world, IRocket rocket)
        {
            return new Mission(
                new DirectTransferMissionTask(world, rocket, new OnGroundState(world, "Kerbin"), new OnGroundState(world, "Duna")),
                new WaitForRealTimeTask(world, 300),
                new DirectTransferMissionTask(world, rocket, new OnGroundState(world, "Duna"), new OnGroundState(world, "Kerbin")),
                new WaitForRealTimeTask(world, 300),
                new NewLaunchTask(world, rocket, randomRocketDesign(world))
            );
        }

        protected Mission newMissionKerbinSystemTour(IWorld world, IRocket rocket)
        {
            return new Mission(
                new RunMissionTask(world, rocket, new OnGroundState(world, "Kerbin"), new OnGroundState(world, "Minmus")),
                new RunMissionTask(world, rocket, new OnGroundState(world, "Minmus"), new OnGroundState(world, "Mun")),
                new RunMissionTask(world, rocket, new OnGroundState(world, "Mun"), new OnGroundState(world, "Kerbin"))
            );
        }

        protected Mission newMissionOrbitAndBack(IWorld world, IRocket rocket)
        {
            return new Mission(
                new RunMissionTask(world, rocket, new OnGroundState(world, "Kerbin"), new InOrbitState(world, "Kerbin")),
                new WaitForRealTimeTask(world, 60),
                new RunMissionTask(world, rocket, new InOrbitState(world, "Kerbin"), new OnGroundState(world, "Kerbin")),
                new WaitForRealTimeTask(world, 60),
                new NewLaunchTask(world, rocket, randomRocketDesign(world))
            );
        }

        private string randomRocketDesign(IWorld world)
        {
            return world.RocketDesigns[random.Next(world.RocketDesigns.Count)];
        }
    }
}
