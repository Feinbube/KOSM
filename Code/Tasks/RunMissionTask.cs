﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Game;
using KOSM.States;

namespace KOSM.Tasks
{
    public class RunMissionTask : Task
    {
        private Rocket rocket;
        private State start;
        private State objective;

        public RunMissionTask(World world, Rocket rocket, State start, State objective)
        {
            this.rocket = rocket;
            this.start = start;
            this.objective = objective;
        }

        public override void Execute(World world, Mission mission)
        {
            if (start is OnGroundState && objective is InOrbitState)
            {
                mission.PushAfter(this, 
                    new RaiseToLowOrbitTask(world, rocket),
                    new CircularizeOrbitTask(world, rocket)
                    );

                mission.Complete(world, this);
            }
            else if (start is InOrbitState && objective is OnGroundState)
            {
                mission.PushAfter(this,
                    new LandAtTask(world, rocket, objective as OnGroundState)
                    );

                mission.Complete(world, this);
            }
            else
                throw new NotImplementedException();
        }

        public override string Description
        {
            get { return "Moving the " + rocket + " from " + start + " to " + objective + "."; }
        }
    }
}
