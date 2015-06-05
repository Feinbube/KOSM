using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KOSM.Game
{
    public class Body : WorldObject
    {
        private CelestialBody celestialBody;

        public string Name { get { return celestialBody.GetName(); } }

        public bool HasAtmosphere { get { return celestialBody.atmosphere; } }

        public double LowOrbit { get { return HasAtmosphere ? celestialBody.atmosphereDepth : Math.Max(10000, celestialBody.Radius * 0.05); } }

        public double SafeLowOrbit { get { return LowOrbit * 1.1; } }

        public double GravityAtSealevel { get { return celestialBody.gravParameter / Math.Pow(celestialBody.Radius, 2); } }

        public Body(World world, CelestialBody celestialBody)
            : base(world)
        {
            this.celestialBody = celestialBody;
        }

        public override bool Equals(object obj)
        {
            return this.Name.Equals((obj as Body).Name);
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
