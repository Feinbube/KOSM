using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Game;
using KOSM.Reporting;

namespace KOSM.Tasks
{
    public class CircularizeOrbitTask : RocketTask
    {
        public CircularizeOrbitTask(World world, Rocket rocket) : base(world, rocket) { }

        public override void Execute(World world, Mission mission)
        {
            rocket.AddApoapsisManeuver(rocket.MainBody.SafeLowOrbit);
            
            this.Details = "Raising periapsis from " + Format.Distance(rocket.Orbit.PeriapsisAltitude) + " to " + Format.Distance(rocket.MainBody.SafeLowOrbit);

            mission.PushAfter(this,
                new ExecuteManeuverTask(world, rocket),
                new CheckAndRepairOrbitTask(world, rocket, rocket.MainBody.SafeLowOrbit)
                );

            mission.Complete(world, this);
        }

        public override string Description
        {
            get { return "Circulizing the orbit."; }
        }
    }
}
