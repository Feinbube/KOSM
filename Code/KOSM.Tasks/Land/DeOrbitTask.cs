using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;

namespace KOSM.Tasks
{
    public class DeOrbitTask : RocketTask
    {
        protected IOnGroundState destination = null;
        public DeOrbitTask(IWorld world, IRocket rocket, IOnGroundState destination)
            : base(world, rocket)
        {
            this.destination = destination;
        }

        public override void Execute(IWorld world, Mission mission)
        {
            if (rocket.Body.HasAtmosphere)
                executeAtmosphere(world, mission);
            else
                executeVacuum(world, mission);
        }

        private void executeAtmosphere(IWorld world, Mission mission)
        {
            if (rocket.Altitude < rocket.Body.SafeLowOrbitAltitude * 0.2)
            {
                mission.Complete(world, this);
                return;
            }

            if (rocket.Orbit.Periapsis.Altitude > rocket.Body.SafeLowOrbitAltitude * 0.6)
            {
                if (!orbitIsHyperbolic && (rocket.Orbit.Apoapsis.MovingTowards || rocket.Orbit.Periapsis.Altitude > rocket.Body.LowOrbitAltitude))
                    rocket.AddApoapsisManeuver(rocket.Body.LowOrbitAltitude * 0.5 + rocket.Body.Radius);
                else
                    rocket.AddPeriapsisManeuver(rocket.Body.LowOrbitAltitude * 0.5 + rocket.Body.Radius);
                mission.PushBefore(this, new ExecuteManeuverTask(world, rocket));
                return;
            }

            rocket.SetSteering(rocket.SurfaceRetrograde);
            rocket.Throttle = 0;
            world.OneTickWarpTimeTo(rocket.Orbit.Periapsis.TimeTill);
        }

        bool first = true;
        private void executeVacuum(IWorld world, Mission mission)
        {
            if (rocket.Altitude < rocket.Body.SafeLowOrbitAltitude * 2)
            {
                mission.Complete(world, this);
                return;
            }

            if (rocket.HorizontalSurfaceSpeed < 3)
            {
                rocket.Throttle = 0;
                Details = "Waiting for landing height.";
                return;
            }

            if (rocket.Altitude > rocket.Body.SafeLowOrbitAltitude * 2 && rocket.Orbit.Periapsis.Altitude > rocket.Body.SafeLowOrbitAltitude * 2.1)
            {
                rocket.AddPeriapsisManeuver(rocket.Body.SafeLowOrbitAltitude * 2 + rocket.Body.Radius);
                mission.PushBefore(this, new ExecuteManeuverTask(world, rocket));
                return;
            }

            if (first)
                if (!TurnAndWait(world, rocket.Orbit.Periapsis.TimeTill, rocket.SurfaceRetrograde))
                    return;
            first = false;

            rocket.SetSteering(rocket.SurfaceRetrograde);
            rocket.Throttle = 1;
        }


        private bool orbitIsHyperbolic
        {
            get { return rocket.Orbit.Apoapsis.Altitude < 0; }
        }

        public override string Description
        {
            get { return "Deorbiting over " + destination + (!rocket.Body.HasAtmosphere ? "" : " with aerobraking") + "."; }
        }
    }
}
