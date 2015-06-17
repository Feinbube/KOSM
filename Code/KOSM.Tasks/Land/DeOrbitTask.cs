using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;

namespace KOSM.Tasks
{
    public class DeOrbitTask : RocketTask
    {
        protected IOnGroundState objective = null;
        public DeOrbitTask(IWorld world, IRocket rocket, IOnGroundState objective)
            : base(world, rocket)
        {
            this.objective = objective;
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

            if (rocket.Orbit.PeriapsisAltitude > rocket.Body.SafeLowOrbitAltitude * 0.6)
            {
                if (rocket.Orbit.MovingTowardsApoapsis || rocket.Orbit.PeriapsisAltitude > rocket.Body.LowOrbitAltitude)
                {
                    rocket.AddApoapsisManeuver(rocket.Body.LowOrbitAltitude * 0.5 + rocket.Body.Radius);
                    mission.PushBefore(this, new ExecuteManeuverTask(world, rocket));
                    return;
                }

                if (rocket.Orbit.MovingTowardsPeriapsis)
                {
                    world.WarpTime(rocket.Orbit.TimeToPeriapsis - 120);
                    rocket.SetSteering(rocket.OrbitRetrograde);

                    if (rocket.Turned)
                        rocket.Throttle = 1;
                    return;
                }
            }

            rocket.SetSteering(rocket.SurfaceRetrograde);
            rocket.Throttle = 0;
            world.WarpTime(rocket.Orbit.TimeToPeriapsis);
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

            if (rocket.Altitude > rocket.Body.SafeLowOrbitAltitude * 2 && rocket.Orbit.PeriapsisAltitude > rocket.Body.SafeLowOrbitAltitude * 2.1)
            {
                rocket.AddPeriapsisManeuver(rocket.Body.SafeLowOrbitAltitude * 2 + rocket.Body.Radius);
                mission.PushBefore(this, new ExecuteManeuverTask(world, rocket));
                return;
            }

            if (first)
                if (!TurnAndWait(world, rocket.Orbit.TimeToPeriapsis, rocket.SurfaceRetrograde))
                    return;
            first = false;

            rocket.SetSteering(rocket.SurfaceRetrograde);
            rocket.Throttle = 1;
        }

        public override string Description
        {
            get { return "Deorbiting over " + objective.ToString() + (!rocket.Body.HasAtmosphere ? "" : " with aerobraking") + "."; }
        }
    }
}
