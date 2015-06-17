﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;

namespace KOSM.Game
{
    public class Maneuver : WorldObject, IManeuver
    {
        private Rocket rocket;
        private ManeuverNode maneuverNode;
        private PatchedConicSolver patchedConicSolver;

        private bool completed = false;

        public Maneuver(World world, Rocket rocket, PatchedConicSolver patchedConicSolver, ManeuverNode maneuverNode)
            : base(world)
        {
            this.world = world;
            this.rocket = rocket;
            this.patchedConicSolver = patchedConicSolver;
            this.maneuverNode = maneuverNode;
        }

        #region WorldObject

        public override string Identifier
        {
            get { return this.TimeOfBurn.ToString(); }
        }

        #endregion WorldObject

        #region IManeuver

        public bool Completed
        {
            get { return completed || BurnVector.Magnitude <= rocket.TurnDeviation; }
        }

        public void Complete()
        {
            completed = true;
            patchedConicSolver.RemoveManeuverNode(maneuverNode);
        }

        public void Abort()
        {
            completed = true;
            patchedConicSolver.RemoveManeuverNode(maneuverNode);
        }

        public IVector3 DeltaV
        {
            get { return v3(maneuverNode.DeltaV); }
        }
        
        public IVector3 BurnVector
        {
            get { return v3(maneuverNode.GetBurnVector(rocket.raw.orbit)); }
        }

        public double BurnDuration
        {
            get { return rocket.MaxAcceleration == 0 ? double.MaxValue : BurnVector.Magnitude / rocket.MaxAcceleration; }
        }

        public double TimeWhenDue
        {
            get { return maneuverNode.UT; }
        }

        public double TimeOfBurn
        {
            get { return TimeWhenDue - BurnDuration / 2; }
        }

        public double TimeOfTurn
        {
            get { return TimeOfBurn - 120; }
        }

        public double TimeTillDue
        {
            get { return TimeWhenDue - world.PointInTime; }
        }

        public double TimeTillBurn
        {
            get { return TimeTillDue - BurnDuration / 2; }
        }

        public double TimeTillTurn
        {
            get { return TimeTillBurn - 120; }
        }

        #endregion IManeuver
    }
}
