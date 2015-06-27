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

        public Orbit(World world, global::Orbit orbit, IOrbiter orbiter, IBody body)
            : base(world)
        {
            this.raw = orbit;
            this.Orbiter = orbiter;
            this.Body = body;
        }

        public override string ToString()
        {
            return "Apoapsis: " + Format.Distance(Apoapsis.Altitude) + " Periapsis: " + Format.Distance(Periapsis.Altitude);
        }

        #region WorldObject

        public override string Identifier
        {
            get { return this.Body.Identifier + "_" + this.Apoapsis.Radius + "_" + this.Inclination + "_" + this.Eccentricity + "_" + this.Periapsis.Radius + raw.vel; }
        }

        #endregion WorldObject

        #region IOrbit

        public IOrbiter Orbiter { get; private set; }

        public IBody Body { get; private set; }

        public IPointInOrbit Apoapsis
        {
            get { return new ApoapsisPoint(world, this); }
        }

        public IPointInOrbit Periapsis
        {
            get { return new PeriapsisPoint(world, this); }
        }

        public IPointInOrbit BodyPrograde
        {
            get { return new BodyProgradePoint(world, this); }
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

        public double Period
        {
            get { return raw.period; }
        }

        public IVectorXYZ VelocityVector
        {
            get { return vXYZ(raw.vel); }
        }

        #endregion IOrbit
    }
}
