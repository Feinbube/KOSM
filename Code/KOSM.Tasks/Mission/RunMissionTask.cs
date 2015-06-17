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
        private IState start;
        private IState objective;

        public RunMissionTask(IWorld world, IRocket rocket, IState start, IState objective)
        {
            this.rocket = rocket;
            this.start = start;
            this.objective = objective;
        }

        public override void Execute(IWorld world, Mission mission)
        {
            if (start is IOnGroundState)
            {
                mission.PushBefore(this,
                    new RaiseToLowOrbitTask(world, rocket),
                    new StabilizeOrbitTask(world, rocket)
                    );
            }

            if (objective is IOnGroundState)
            {
                mission.PushAfter(this,
                    new LandAtTask(world, rocket, objective as IOnGroundState)
                    );
            }

            IBody commonMainBody = findCommonMainBody(start.Body, objective.Body);
            
            IBody currentBody = start.Body;
            while (commonMainBody != null && currentBody != commonMainBody)
            { // up
                mission.PushBefore(this,
                    new LeaveSphereOfInfluenceTask(world, rocket)
                    );
                currentBody = currentBody.MainBody;
            }

            currentBody = objective.Body;
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

        private IBody findCommonMainBody(IBody start, IBody objective)
        {
            while(start != null)
            {
                if (objective.IsOrbiting(start))
                    return start;
                start = start.MainBody;
            }
            return null;
        }

        public override string Description
        {
            get { return "Running a mission to move " + rocket + " from " + start.ToString() + " to " + objective.ToString() + "."; }
        }
    }
}
