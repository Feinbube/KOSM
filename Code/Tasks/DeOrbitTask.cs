using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Game;
using KOSM.States;

namespace KOSM.Tasks
{
    public class DeOrbitTask : RocketTask
    {
        protected OnGroundState objective = null;
        public DeOrbitTask(World world, Rocket rocket, OnGroundState objective)
            : base(world, rocket)
        {
            this.objective = objective;
        }

        public override void Execute(World world, Mission mission)
        {
            if (rocket.MainBody.HasAtmosphere)
                executeAtmosphere(world, mission);
            else
                executeVacuum(world, mission);
        }

        private void executeAtmosphere(World world, Mission mission)
        {
            if (rocket.Altitude < rocket.MainBody.SafeLowOrbit * 0.125)
            {
                mission.Complete(world, this);
                return;
            }

            if (rocket.Orbit.PeriapsisAltitude > rocket.MainBody.SafeLowOrbit * 0.6)
            {
                if (rocket.Orbit.MovingTowardsApoapsis || rocket.Orbit.PeriapsisAltitude > rocket.MainBody.LowOrbit)
                {
                    rocket.AddApoapsisManeuver(rocket.MainBody.LowOrbit * 0.5);
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
        private void executeVacuum(World world, Mission mission)
        {
            if (rocket.Altitude < rocket.MainBody.SafeLowOrbit * 2)
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

            if (rocket.Altitude > rocket.MainBody.SafeLowOrbit * 2 && rocket.Orbit.PeriapsisAltitude > rocket.MainBody.SafeLowOrbit * 2.1)
            {
                rocket.AddPeriapsisManeuver(rocket.MainBody.SafeLowOrbit * 2);
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
            get { return "Deorbiting over " + objective.ToString() + (!rocket.MainBody.HasAtmosphere ? "" : " with aerobraking") + "."; }
        }
    }
}
