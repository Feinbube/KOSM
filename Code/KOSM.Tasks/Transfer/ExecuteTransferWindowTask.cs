using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;
using KOSM.Common;

namespace KOSM.Tasks
{
    public class ExecuteTransferWindowTask : RocketTask
    {
        ITransferWindow transferWindow = null;

        public ExecuteTransferWindowTask(IWorld world, IRocket rocket, ITransferWindow transferWindow)
            : base(world, rocket)
        {
            this.transferWindow = transferWindow;
        }

        public override void Execute(IWorld world, Mission mission)
        {
            if (!rocket.Orbit.IsCircular)
            {
                mission.PushBefore(this, new CircularizeOrbitTask(world, rocket));
                return;
            }

            transferWindow = rocket.NextTransferWindow(world.PointInTime, rocket.Body, rocket.Orbit.SemiMajorAxis - rocket.Body.Radius, transferWindow.Destination, transferWindow.Aerobraking);

            Details = "Transfer requires a DeltaV of " + Format.Speed(transferWindow.EjectionBurnVector.Magnitude) + ".";

            if (transferWindow.TimeTill > rocket.Orbit.Period)
            {
                mission.PushBefore(this, new WarpTask(world, transferWindow.TimeTill, true));
                return;
            }

            double timeOfManeuver = world.PointInTime + rocket.Orbit.BodyPrograde.TimeTillDegreesToEquals(transferWindow.EjectionAngle);

            rocket.AddManeuver(timeOfManeuver, transferWindow.EjectionBurnVector);
            mission.PushAfter(this,
                new ExecuteManeuverTask(world, rocket, (w, r) => r.HasEncounter && r.NextEncounter.Body.Name == transferWindow.Destination.MainBody.Name),
                new WarpTill(world, rocket, (w, r) => r.Body != transferWindow.Origin, (w, r) => r.Body.SphereOfInfluenceAltitude - r.Altitude, "Warping to leave sphere of influence of " + transferWindow.Origin.Name + "."),
                new AdjustEncounterTask(world, rocket),
                new WarpTask(world, transferWindow.TravelTime / 2, true),
                new WarpTill(world, rocket, (w, r) => r.Body == transferWindow.Destination, (w, r) => transferWindow.Destination.Position.Minus(r.Position).Magnitude, "Warping to encounter with " + transferWindow.Destination.Name + "."),
                new WarpTask(world, 2, true) // wait some time to ensure the game understands that we are in the encounter now
                );

            mission.Complete(world, this);
        }

        public override string Description
        {
            get { return "Executing transfer from " + transferWindow.Origin + " to " + transferWindow.Destination + "."; }
        }
    }
}
