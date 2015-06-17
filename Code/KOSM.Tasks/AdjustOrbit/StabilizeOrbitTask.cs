using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;
using KOSM.Common;

namespace KOSM.Tasks
{
    public class StabilizeOrbitTask : RocketTask
    {
        public StabilizeOrbitTask(IWorld world, IRocket rocket) : base(world, rocket) { }

        public override void Execute(IWorld world, Mission mission)
        {
            rocket.AddApoapsisManeuver(rocket.Body.SafeLowOrbitRadius);
            
            this.Details = "Raising periapsis from " + Format.Distance(rocket.Orbit.PeriapsisAltitude) + " to " + Format.Distance(rocket.Body.SafeLowOrbitAltitude);

            mission.PushAfter(this,
                new ExecuteManeuverTask(world, rocket),
                new CheckAndRepairOrbitTask(world, rocket, rocket.Body.SafeLowOrbitAltitude)
                );

            mission.Complete(world, this);
        }

        public override string Description
        {
            get { return "Stabilizing the orbit."; }
        }
    }
}
