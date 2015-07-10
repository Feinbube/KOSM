using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;
using KOSM.Common;

namespace KOSM.Tasks
{
    public class AdjustEncounterTask : RocketTask
    {
        double minEncounter = double.MaxValue;

        public AdjustEncounterTask(IWorld world, IRocket rocket)
            : base(world, rocket)
        {
        }

        public override void Execute(IWorld world, Mission mission)
        {
            if (!rocket.HasEncounter)
            {
                rocket.SetSteering(rocket.OrbitPrograde);
                rocket.Throttle = rocket.TurnDeviation > 0.03 ? 0 : 0.5 / rocket.MaxAcceleration;
                return;
            }

            if (rocket.NextEncounter.Periapsis.Altitude <= rocket.NextEncounter.Body.SafeLowOrbitAltitude / 2 || rocket.NextEncounter.Periapsis.Altitude > minEncounter)
            {
                rocket.Throttle = 0;
                mission.Complete(world, this);
                return;
            }

            minEncounter = rocket.NextEncounter.Periapsis.Altitude;

            rocket.SetSteering(rocket.OrbitPrograde);
            rocket.Throttle = rocket.Throttle = rocket.TurnDeviation > 0.03 ? 0 :
                1 / rocket.MaxAcceleration * Math.Max(0.25, (rocket.NextEncounter.Periapsis.Altitude - rocket.NextEncounter.Body.SafeLowOrbitAltitude / 2) / (rocket.NextEncounter.Periapsis.Altitude));
        }

        public override string Description
        {
            get
            {
                return !rocket.HasEncounter ? "Adjusting next encounter."
                    : "Adjusting encounter with " + rocket.NextEncounter.Body.Name + " for a periapsis of " + Format.Distance(rocket.NextEncounter.Body.SafeLowOrbitAltitude / 2) + ".";
            }
        }
    }
}
