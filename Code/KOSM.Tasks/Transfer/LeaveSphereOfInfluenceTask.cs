using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;
using KOSM.Common;

namespace KOSM.Tasks
{
    public class LeaveSphereOfInfluenceTask : RocketTask
    {
        public LeaveSphereOfInfluenceTask(IWorld world, IRocket rocket) : base(world, rocket) { }

        public override void Execute(IWorld world, Mission mission)
        {
            rocket.AddPeriapsisManeuver(rocket.Body.SphereOfInfluenceRadius);

            this.Details = "Raising apoapsis from " + Format.Distance(rocket.Orbit.Apoapsis.Altitude) + " to " + Format.Distance(rocket.Body.SphereOfInfluenceAltitude);

            mission.PushAfter(this,
                new ExecuteManeuverTask(world, rocket)
                );

            mission.Complete(world, this);
        }

        public override string Description
        {
            get { return "Leaving the influence of " + rocket.Body.Name + "."; }
        }
    }
}
