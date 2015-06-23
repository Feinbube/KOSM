using System;

namespace KOSM.Interfaces
{
    public interface IOrbit
    {
        IBody Body { get; }

        IPointInOrbit Apoapsis { get; }
        IPointInOrbit Periapsis { get; }

        double SemiMajorAxis { get; }

        double Eccentricity { get; }
        double Inclination { get; }

        bool IsCircular { get; }
    }
}
