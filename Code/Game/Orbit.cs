using KOSM.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KOSM.Game
{
    public class Orbit : WorldObject
    {
        internal global::Orbit orbit = null;

        public double ApoapsisAltitude { get { return this.orbit.ApA; } }

        public double ApoapsisRadius { get { return this.orbit.ApR; } }

        public double TimeToApoapsis { get { return orbit.timeToAp; } }

        public double VelocityAtApoapsis { get { return OrbitVelocity(orbit.referenceBody.gravParameter, ApoapsisRadius, orbit.semiMajorAxis); } }

        public double VelocityAtPeriapsis { get { return OrbitVelocity(orbit.referenceBody.gravParameter, PeriapsisRadius, orbit.semiMajorAxis); } }

        public double PeriapsisAltitude { get { return this.orbit.PeA; } }

        public double PeriapsisRadius { get { return this.orbit.PeA; } }

        public double TimeToPeriapsis { get { return orbit.timeToPe; } }

        public Orbit(World world, global::Orbit orbit)
            : base(world)
        {
            this.orbit = orbit;
        }

        public double DeltaVForApoapsisManeuver(double targetAltitude)
        {
            double targetVelocityAtApsis = OrbitVelocity(
                orbit.referenceBody.gravParameter,
                ApoapsisRadius,
                SemiMajorAxis(orbit.referenceBody.Radius, ApoapsisAltitude, targetAltitude)
                );

            return targetVelocityAtApsis - VelocityAtApoapsis;
        }

        public double DeltaVForPeriapsisManeuver(double targetAltitude)
        {
            double targetVelocityAtApsis = OrbitVelocity(
                orbit.referenceBody.gravParameter,
                PeriapsisRadius,
                SemiMajorAxis(orbit.referenceBody.Radius, targetAltitude, PeriapsisAltitude)
                );

            return targetVelocityAtApsis - VelocityAtPeriapsis;
        }

        // v = sqrt( G*M * (2/r - 1/a) )
        public double OrbitVelocity(double gravParameter, double radius, double semiMajorAxis)
        {
            return Math.Sqrt(gravParameter * (2 / radius - 1 / semiMajorAxis));
        }

        // a = (r1 + r2) / 2 ///// a is semi major axis here
        public double SemiMajorAxis(double radiusAtApoapsis, double radiusAtPeriapsis)
        {
            return 0.5 * (radiusAtApoapsis + radiusAtPeriapsis);
        }

        public double SemiMajorAxis(double bodyRadius, double altitudeAtApoapsis, double altitudeAtPeriapsis)
        {
            return SemiMajorAxis(bodyRadius + altitudeAtApoapsis, bodyRadius + altitudeAtPeriapsis);
        }

        public override string ToString()
        {
            return "Apoapsis: " + Format.Distance(ApoapsisAltitude) + " Periapsis: " + Format.Distance(PeriapsisAltitude);
        }

        public bool MovingTowardsPeriapsis { get { return TimeToApoapsis > TimeToPeriapsis; } }

        public bool MovingTowardsApoapsis { get { return TimeToApoapsis < TimeToPeriapsis; } }
    }
}
