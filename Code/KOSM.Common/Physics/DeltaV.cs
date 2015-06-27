using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;

namespace KOSM.Common
{
    public static class DeltaV
    {
        public static IVectorXYZ ApoapsisManeuver(IOrbit orbit, double targetPeriapsisRadius)
        {
            return new VectorXYZ(0, 0, Velocity.OrbitVelocity(orbit.Body.GravityParameter, orbit.Apoapsis.Radius, targetPeriapsisRadius, orbit.Apoapsis.Radius) - orbit.Apoapsis.Velocity);
        }

        public static IVectorXYZ PeriapsisManeuver(IOrbit orbit, double targetApoapsisRadius)
        {
            return new VectorXYZ(0, 0, Velocity.OrbitVelocity(orbit.Body.GravityParameter, orbit.Periapsis.Radius, orbit.Periapsis.Radius, targetApoapsisRadius) - orbit.Periapsis.Velocity);
        }
        
        //public double DeltaVForApoapsisManeuver(double targetAltitude)
        //{
        //    double targetVelocityAtApsis = OrbitVelocity(
        //        raw.referenceBody.gravParameter,
        //        Apoapsis.Radius,
        //        SemiMajorAxis(raw.referenceBody.Radius, Apoapsis.Altitude, targetAltitude)
        //        );

        //    return targetVelocityAtApsis - VelocityAtApoapsis;
        //}

        //public static double DeltaVToOrbiterInclination(double parkingRadiusOfCircularOrbit, IBody orbiter)
        //{
        //    //double di = Math.Abs(this.Orbit.Inclination - orbiter.Orbit.Inclination);
        //    //double w = this.orbit.raw.argumentOfPeriapsis;
        //    //double f = this.orbit.raw.trueAnomaly;
        //    //double n = 1;
        //    //double a = this.orbit.raw.semiMajorAxis;

        //    //return 2 * Math.Sin(di / 2) * Math.Cos(w + f) * n * a / (1 + Math.Cos(f));
        //    return 0;
        //}

        //// http://en.wikipedia.org/wiki/Orbital_inclination_change
        //public static double DeltaVToOrbiterInclination(IBody orbiter)
        //{
        //    //double di = Math.Abs(this.Orbit.Inclination - orbiter.Orbit.Inclination);
        //    //double e = this.orbit.Eccentricity;
        //    //double w = this.orbit.raw.argumentOfPeriapsis;
        //    //double f = this.orbit.raw.trueAnomaly;
        //    //double n = 1;
        //    //double a = this.orbit.raw.semiMajorAxis;

        //    //return 2 * Math.Sin(di / 2) * Math.Sqrt(1 - e * e) * Math.Cos(w + f) * n * a / (1 + e * Math.Cos(f));
        //    return 0;
        //}

        //public static double DeltaVSurfaceToLowOrbit { get { return LowOrbitXVelocity; } }

        //public static double DeltaVLowOrbitToEscape { get { return orbit.OrbitVelocity(raw.gravParameter, LowOrbitRadius, orbit.SemiMajorAxis(raw.sphereOfInfluence, LowOrbitRadius)) - LowOrbitXVelocity; } }

        //public static double DeltaVCaptureToLowOrbit
        //{
        //    get
        //    {
        //        return SOIEdgeVelocity - orbit.OrbitVelocity(raw.gravParameter, raw.sphereOfInfluence, orbit.SemiMajorAxis(raw.sphereOfInfluence, LowOrbitRadius));
        //    }
        //}
    }
}
