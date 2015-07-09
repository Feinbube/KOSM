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
            if(!rocket.Orbit.IsCircular)
            {
                mission.PushBefore(this, new CircularizeOrbitTask(world, rocket));
                return;
            }

            transferWindow = rocket.NextTransferWindow(world.PointInTime, rocket.Body, rocket.Orbit.SemiMajorAxis - rocket.Body.Radius, transferWindow.Destination, transferWindow.Aerobraking);

            if(transferWindow.TimeTill > rocket.Orbit.Period)
            {
                mission.PushBefore(this, new WarpTask(world, transferWindow.TimeTill));
                return;
            }

            double timeOfManeuver = world.PointInTime + rocket.Orbit.BodyPrograde.TimeTillDegreesToEquals(transferWindow.EjectionAngle);

            rocket.AddManeuver(timeOfManeuver, transferWindow.EjectionBurnVector);
            mission.PushAfter(this, 
                new ExecuteManeuverTask(world, rocket), 
                new WarpTask(world, transferWindow.TravelTime / 2),
                
                // TODO: control when approaching (to get closer and hit the atmosphere just right)

                new WarpToEncounter(world, rocket, transferWindow.Destination));

            mission.Complete(world, this);
        }

        public override string Description
        {
            get { return "Executing transfer from " + transferWindow.Origin + " to " + transferWindow.Destination + " with a DeltaV of " + Format.Speed(transferWindow.EjectionBurnVector.Magnitude) + "." ; }
        }
    }
}
