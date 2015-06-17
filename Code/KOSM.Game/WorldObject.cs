using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;
using KOSM.Common;

namespace KOSM.Game
{
    public abstract class WorldObject : IIdentifiable
    {
        protected World world = null;

        protected WorldObject(World world)
        {
            this.world = world;
        }

        public abstract string Identifier { get; }

        public override bool Equals(object obj)
        {
            return this.Identifier.Equals((obj as WorldObject).Identifier);
        }

        public override int GetHashCode()
        {
            return this.Identifier.GetHashCode();
        }

        #region Shortcut Functions

        protected double clamp(double value, double min, double max)
        {
            return Math.Min(max, Math.Max(min, value));
        }

        protected void dLog(string message)
        {
            world.LiveDebugLog.Add(message);
        }
        protected void mLog(string message)
        {
            world.MissionLog.Add(message);
        }

        protected IVector3 v3(Vector3d vector)
        {
            return new Vector3(vector.x, vector.y, vector.z);
        }

        protected Vector3d v3d(IVector3 vector)
        {
            return new Vector3d(vector.X, vector.Y, vector.Z);
        }

        #endregion Shortcut Functions
    }
}
