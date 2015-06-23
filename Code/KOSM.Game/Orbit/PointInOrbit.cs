using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;

namespace KOSM.Game
{
    public abstract class PointInOrbit : IPointInOrbit
    {
        protected World world = null;
        protected Orbit orbit = null;

        public PointInOrbit(World world, Orbit orbit)
        {
            this.world = world;
            this.orbit = orbit;
        }

        public abstract double Altitude {get; }
        public abstract double Radius { get; }
        public abstract double TimeTill { get; }
        public double TimeOf
        {
            get { return world.PointInTime + TimeTill; }
        }

        public abstract bool MovingTowards { get; }

        public double Velocity
        {
            get { return KOSM.Common.Velocity.OrbitVelocity(orbit, this.Radius); }
        }
    }
}
