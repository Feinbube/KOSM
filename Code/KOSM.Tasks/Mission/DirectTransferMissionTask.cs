using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Common;
using KOSM.Interfaces;

namespace KOSM.Tasks
{
    public class DirectTransferMissionTask : Task
    {
        private IRocket rocket;
        private IState origin;
        private IState destination;

        public DirectTransferMissionTask(IWorld world, IRocket rocket, IState origin, IState destination)
        {
            this.rocket = rocket;
            this.origin = origin;
            this.destination = destination;
        }

        public override void Execute(IWorld world, Mission mission)
        {
            if (origin is IOnGroundState)
            {
                double timeToOrbit = Format.KerbinTimespanTotalSeconds(0, 0, 2, 0, 0); // two hours should be enough to put the craft into orbit on time
                ITransferWindow transferWindow = rocket.NextTransferWindow(world.PointInTime + timeToOrbit, origin.Body, destination.Body, destination.Body.HasAtmosphere);
                
                mission.PushBefore(this,
                    new WarpTask(world, transferWindow.TimeTill - timeToOrbit),
                    new RaiseToLowOrbitTask(world, rocket),
                    new CircularizeOrbitTask(world, rocket),
                    new ExecuteTransferWindowTask(world, rocket, transferWindow)
                    );
            }
            else
            {
                ITransferWindow transferWindow = rocket.NextTransferWindow(world.PointInTime, origin.Body, destination.Body, destination.Body.HasAtmosphere);
                mission.PushBefore(this, new ExecuteTransferWindowTask(world, rocket, transferWindow));
            }

            if (destination is IOnGroundState)
            {
                mission.PushAfter(this,
                    new LandAtTask(world, rocket, destination as IOnGroundState)
                    );
            }

            mission.Complete(world, this);
        }

        public override string Description
        {
            get { return "Running a mission to transfer " + rocket + " from " + origin + " to " + destination + "."; }
        }
    }
}
