using System;
using System.Collections.Generic;

namespace KOSM.Interfaces
{
    public interface IBody : IIdentifiable
    {
        string Name { get; }
        bool HasAtmosphere { get; }
        double AtmosphereHeight { get; }

        IBody MainBody { get; }
        IOrbit Orbit { get; }

        List<IBody> Moons { get; }
        bool IsOrbiting(IBody body);

        double Radius { get; }
        double LowOrbitAltitude { get; }
        double LowOrbitRadius { get; }
        double SafeLowOrbitAltitude { get; }
        double SafeLowOrbitRadius { get; }
        double SphereOfInfluenceAltitude { get; }
        double SphereOfInfluenceRadius { get; }

        double GravityParameter { get; }
        double GravityAtSealevel { get; }        
    }
}
