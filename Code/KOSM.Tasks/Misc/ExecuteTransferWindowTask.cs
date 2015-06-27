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
            double timeOfManeuver = world.PointInTime + rocket.Orbit.BodyPrograde.TimeTillDegreesToEquals(transferWindow.EjectionAngle);

            rocket.AddManeuver(timeOfManeuver, transferWindow.EjectionBurnVector);
            mission.PushAfter(this, new ExecuteManeuverTask(world, rocket));

            // TODO: Insertion

            mission.Complete(world, this);
        }

        public override string Description
        {
            get { return "Executing transfer from " + transferWindow.Origin + " to " + transferWindow.Destination + " with a DeltaV of " + Format.Speed(transferWindow.EjectionBurnVector.Magnitude) + "." ; }
        }
    }
}
