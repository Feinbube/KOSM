using System;

namespace KOSM.Interfaces
{
    public interface IOrbit
    {
        IBody Body { get; }

        double ApoapsisAltitude { get; }
        double ApoapsisRadius { get; }
        double TimeToApoapsis { get; }
        bool MovingTowardsApoapsis { get; }
        double VelocityAtApoapsis { get; }
        IVector3 DeltaVForApoapsisManeuver(double targetPeriapsisRadius);
        
        double PeriapsisAltitude { get; }
        double PeriapsisRadius { get; }
        double TimeToPeriapsis { get; }
        bool MovingTowardsPeriapsis { get; }
        double VelocityAtPeriapsis { get; }
        IVector3 DeltaVForPeriapsisManeuver(double targetApoapsisRadius);

        double SemiMajorAxis { get; }

        double Eccentricity { get; }
        double Inclination { get; }

        bool IsCircular { get; }
    }
}
