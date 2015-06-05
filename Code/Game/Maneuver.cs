using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KOSM.Game
{
    public class Maneuver
    {
        private World world;
        private Rocket rocket;
        private ManeuverNode maneuverNode;
        private PatchedConicSolver patchedConicSolver;

        public Vector3d InitialDeltaV { get; private set; }
        public double InitialMagnitude { get { return InitialDeltaV.magnitude; } }

        public Vector3d DeltaV { get { return maneuverNode.DeltaV; } }
        public Vector3d BurnVector { get { return maneuverNode.GetBurnVector(rocket.Orbit.orbit); } }        
        public double Magnitude { get { return BurnVector.magnitude; } }

        public Maneuver(World world, Rocket rocket, PatchedConicSolver patchedConicSolver, ManeuverNode maneuverNode)
        {
            this.world = world;
            this.rocket = rocket;
            this.patchedConicSolver = patchedConicSolver;
            this.maneuverNode = maneuverNode;

            this.InitialDeltaV = maneuverNode.DeltaV;
        }

        public double TimeWhenDue { get { return maneuverNode.UT; } }
        public double TimeOfBurn { get { return TimeWhenDue - BurnDuration / 2; } }
        public double TimeOfTurn { get { return TimeOfBurn - 60; } }

        public double TimeTillDue { get { return TimeWhenDue - world.PointInTime; } }
        public double TimeTillBurn { get { return TimeTillDue - BurnDuration / 2; } }
        public double TimeTillTurn { get { return TimeTillBurn - 60; } }

        public double MissionTimeWhenDue { get { return rocket.MissionTime + TimeTillDue; } }

        public double BurnDuration
        {
            get
            {
                return rocket.MaxAcceleration == 0 ? double.MaxValue : this.Magnitude / rocket.MaxAcceleration;
            }
        }

        public void Remove()
        {
            patchedConicSolver.RemoveManeuverNode(maneuverNode);
        }

        public bool Complete { get { return Vector3d.Dot(InitialDeltaV, DeltaV) < 0 || BurnVector.magnitude < 0.1; } }        
    }
}
