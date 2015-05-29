using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KOSM.Game
{
    public class Orbit : WorldObject
    {
        global::Orbit orbit = null;

        public double ApoapsisAltitude { get { return this.orbit.ApA; } }
        public double ApoapsisRadius { get { return this.orbit.ApR; } }

        public double PeriapsisAltitude { get { return this.orbit.PeA; } }
        public double PeriapsisRadius { get { return this.orbit.PeA; } }

        public Orbit(World world, global::Orbit orbit)
            : base(world)
        {
            this.orbit = orbit;
        }
    }
}
