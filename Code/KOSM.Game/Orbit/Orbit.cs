using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;
using KOSM.Common;

namespace KOSM.Game
{
    public class Orbit : WorldObject, IOrbit
    {
        internal global::Orbit raw = null;

        public IBody Body { get; private set; }

        public Orbit(World world, global::Orbit orbit, IBody body)
            : base(world)
        {
            this.raw = orbit;
            this.Body = body;
        }

        public override string ToString()
        {
            return "Apoapsis: " + Format.Distance(Apoapsis.Altitude) + " Periapsis: " + Format.Distance(Periapsis.Altitude);
        }

        #region WorldObject

        public override string Identifier
        {
            get { return this.Body.Identifier + "_" + this.Apoapsis.Radius.ToString() + "_" + this.Inclination + "_" + this.Eccentricity + "_" + this.Periapsis.Radius.ToString() + raw.vel ; }
        }

        #endregion WorldObject

        #region IOrbit       

        public IPointInOrbit Apoapsis
        {
            get { return new ApoapsisPoint(world, this); }
        }

        public IPointInOrbit Periapsis
        {
            get { return new PeriapsisPoint(world, this); }
        }

        public double SemiMajorAxis
        {
            get { return raw.semiMajorAxis; }
        }

        public double Eccentricity
        {
            get { return raw.eccentricity; }
        }

        public double Inclination
        {
            get { return Inclination; }
        }

        public bool IsCircular
        {
            get
            {
                return Math.Abs(this.Periapsis.Altitude - this.Apoapsis.Altitude) < this.Body.LowOrbitAltitude * 0.02;
            }
        }

        #endregion IOrbit
    }
}
