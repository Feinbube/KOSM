using System;

namespace KOSM.Interfaces
{
    public interface IOrbit
    {
        IOrbiter Orbiter { get; }
        IBody Body { get; }

        IPointInOrbit Apoapsis { get; }
        IPointInOrbit Periapsis { get; }
        IPointInOrbit BodyPrograde { get; }

        double SemiMajorAxis { get; }

        double Eccentricity { get; }
        double Inclination { get; }

        bool IsCircular { get; }

        double Period { get; }

        IVectorXYZ VelocityVector { get; }
    }
}
