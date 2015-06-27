using System;
using System.Collections.Generic;

namespace KOSM.Interfaces
{
    public interface IRocket : IRocketControl, IRocketManeuvers, IOrbiter, IIdentifiable
    {
        string Name { get; }

        IBody Body { get; }
        IOrbit Orbit { get; }

        double RocketVerticalHeight { get; }
        double Mass { get; }

        // interaction        
        void RaiseGear();
        void LowerGear();

        // status
        double MissionTime { get; }
        bool Landed { get; }
        bool InCircularOrbit { get; }

        double Altitude { get; }
        double AltitudeOverGround { get; }

        // speed
        double VerticalSpeed { get; }
        double HorizontalSurfaceSpeed { get; }
        IVectorXYZ SurfaceVelocity { get; }

        double TimeToMissionTime(double pointInTime);        
    }
}
