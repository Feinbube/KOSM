using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;

namespace KOSM.Tasks
{
    public class RunMissionTask : Task
    {
        private IRocket rocket;
        private IState origin;
        private IState destination;

        public RunMissionTask(IWorld world, IRocket rocket, IState origin, IState destination)
        {
            this.rocket = rocket;
            this.origin = origin;
            this.destination = destination;
        }

        public override void Execute(IWorld world, Mission mission)
        {
            if (origin is IOnGroundState)
            {
                mission.PushBefore(this,
                    new RaiseToLowOrbitTask(world, rocket),
                    new StabilizeOrbitTask(world, rocket)
                    );
            }

            if (destination is IOnGroundState)
            {
                mission.PushAfter(this,
                    new LandAtTask(world, rocket, destination as IOnGroundState)
                    );
            }

            IBody commonMainBody = findCommonMainBody(origin.Body, destination.Body);
            
            IBody currentBody = origin.Body;
            while (commonMainBody != null && currentBody != commonMainBody)
            { // up
                mission.PushBefore(this,
                    new LeaveSphereOfInfluenceTask(world, rocket)
                    );
                currentBody = currentBody.MainBody;
            }

            currentBody = destination.Body;
            while (commonMainBody != null && currentBody != commonMainBody)
            { // down
                mission.PushAfter(this,
                    new HohmannTransferTask(world, rocket, currentBody),
                    new StabilizeOrbitTask(world, rocket)
                    );
                currentBody = currentBody.MainBody;
            }

            mission.Complete(world, this);
        }

        private IBody findCommonMainBody(IBody origin, IBody destination)
        {
            while(origin != null)
            {
                if (destination.IsOrbiting(origin))
                    return origin;
                origin = origin.MainBody;
            }
            return null;
        }

        public override string Description
        {
            get { return "Running a mission to move " + rocket + " from " + origin + " to " + destination + "."; }
        }
    }
}
