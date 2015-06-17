using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;

namespace KOSM.Common
{
    public static class Velocity
    {
        public static double OrbitVelocity(IOrbit orbit, double radius)
        {
            return OrbitVelocity(orbit.Body.GravityParameter, radius, orbit.SemiMajorAxis);
        }

        public static double OrbitVelocity(double gravParameter, double radius, double periapsisRadius, double apoapsisRadius)
        {
            return OrbitVelocity(gravParameter, radius, 0.5 * (apoapsisRadius + periapsisRadius));
        }

        // v = sqrt( G*M * (2/r - 1/a) )
        public static double OrbitVelocity(double gravParameter, double radius, double semiMajorAxis)
        {
            return Math.Sqrt(gravParameter * (2.0 / radius - 1.0 / semiMajorAxis));
        }

        public static double AtApoapsis(IOrbit orbit)
        {
            return OrbitVelocity(orbit, orbit.ApoapsisRadius);
        }

        public static double AtPeriapsis(IOrbit orbit)
        {
            return OrbitVelocity(orbit, orbit.PeriapsisRadius);
        }
        

        //public static double SOIEdgeVelocity { get { return orbit.OrbitVelocity(raw.gravParameter, raw.sphereOfInfluence, orbit.SemiMajorAxis(raw.sphereOfInfluence, raw.sphereOfInfluence)); } }

        //public static double MeanMotion(IOrbit o)
        //{
        //    if (o.raw.eccentricity > 1)
        //    {
        //        return Math.Sqrt(o.raw.referenceBody.gravParameter / Math.Abs(Math.Pow(o.raw.semiMajorAxis, 3)));
        //    }
        //    else
        //    {
        //        // The above formula is wrong when using the RealSolarSystem mod, which messes with orbital periods.
        //        // This simpler formula should be foolproof for elliptical orbits:
        //        return 2 * Math.PI / o.raw.period;
        //    }
        //}


        //public static double LowOrbitXVelocity
        //{
        //    get
        //    {
        //        return orbit.OrbitVelocity(raw.gravParameter, LowOrbitRadius, orbit.SemiMajorAxis(LowOrbitRadius, LowOrbitRadius));
        //    }
        //}
    }
}
