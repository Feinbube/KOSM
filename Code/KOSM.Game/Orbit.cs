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
            return "Apoapsis: " + Format.Distance(ApoapsisAltitude) + " Periapsis: " + Format.Distance(PeriapsisAltitude);
        }

        #region WorldObject

        public override string Identifier
        {
            get { return this.Body.Identifier + "_" + this.ApoapsisRadius.ToString() + "_" + this.Inclination + "_" + this.Eccentricity + "_" + this.PeriapsisRadius.ToString() + raw.vel ; }
        }

        #endregion WorldObject

        #region IOrbit       

        public double ApoapsisAltitude
        {
            get { return this.raw.ApA; }
        }

        public double ApoapsisRadius
        {
            get { return this.raw.ApR; }
        }

        public double TimeToApoapsis
        {
            get { return raw.timeToAp; }
        }

        public bool MovingTowardsApoapsis
        {
            get { return TimeToApoapsis < TimeToPeriapsis; }
        }

        public double VelocityAtApoapsis
        {
            get { return Velocity.OrbitVelocity(this, ApoapsisRadius); }
        }

        public IVector3 DeltaVForApoapsisManeuver(double targetPeriapsisRadius)
        {
            return DeltaV.ApoapsisManeuver(this, targetPeriapsisRadius);
        }

        public double PeriapsisAltitude
        {
            get { return this.raw.PeA; }
        }

        public double PeriapsisRadius
        {
            get { return this.raw.PeR; }
        }

        public double TimeToPeriapsis
        {
            get { return raw.timeToPe; }
        }

        public bool MovingTowardsPeriapsis
        {
            get { return TimeToApoapsis > TimeToPeriapsis; }
        }

        public double VelocityAtPeriapsis
        {
            get { return Velocity.OrbitVelocity(this, PeriapsisRadius); }
        }

        public IVector3 DeltaVForPeriapsisManeuver(double targetApoapsisRadius)
        {
            return DeltaV.PeriapsisManeuver(this, targetApoapsisRadius);
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
                return Math.Abs(this.PeriapsisAltitude - this.ApoapsisAltitude) < this.Body.LowOrbitAltitude * 0.02;
            }
        }

        #endregion IOrbit
    }
}
