using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;
using KOSM.Common;

namespace KOSM.Game
{
    public class Body : WorldObject, IBody
    {
        internal CelestialBody raw;

        public Body(World world, CelestialBody celestialBody)
            : base(world)
        {
            this.raw = celestialBody;
        }

        public override string ToString()
        {
            return this.Name;
        }

        #region WorldObject

        public override string Identifier
        {
            get { return this.Name; }
        }

        #endregion WorldObject

        #region IBody

        public string Name
        {
            get { return raw.GetName() == "Sun" ? "Kerbol" : raw.GetName(); }
        }

        public bool HasAtmosphere
        {
            get { return raw.atmosphere; }
        }

        public double AtmosphereHeight
        {
            get { return raw.atmosphereDepth; }
        }

        public IBody MainBody
        {
            get { return world.FindBodyByName(raw.referenceBody.GetName()); }
        }

        public IOrbit Orbit { get { return new Orbit(world, raw.GetOrbit(), this.MainBody); } }

        public List<IBody> Moons
        {
            get { return raw.orbitingBodies.Select(a => world.FindBodyByName(a.GetName())).ToList(); }
        }

        public bool IsOrbiting(IBody body)
        {
            IBody center = this.MainBody;
            while (center != null)
            {
                if (center == body)
                    return true;
                center = center.MainBody;
            }
            return false;
        }

        public double Radius
        {
            get { return raw.Radius; }
        }

        public double LowOrbitAltitude
        {
            get { return HasAtmosphere ? raw.atmosphereDepth : raw.Radius * 0.05; }
        }

        public double LowOrbitRadius
        {
            get { return LowOrbitAltitude + Radius; }
        }

        public double SafeLowOrbitAltitude
        {
            get { return 10000 + LowOrbitAltitude; }
        }

        public double SafeLowOrbitRadius
        {
            get { return SafeLowOrbitAltitude + Radius; }
        }

        public double SphereOfInfluenceAltitude
        {
            get { return raw.sphereOfInfluence - Radius; ; }
        }

        public double SphereOfInfluenceRadius
        {
            get { return raw.sphereOfInfluence; ; }
        }

        public double GravityParameter
        {
            get { return raw.gravParameter; }
        }

        public double GravityAtSealevel
        {
            get { return Gravity.GravityAtSealevel(this); }
        }

        public IVector3 Position
        {
            get { return v3(raw.position); }
        }

        #endregion IBody
    }
}
