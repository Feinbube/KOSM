using System;
using System.Collections.Generic;

namespace KOSM.Interfaces
{
    public interface IRocket : IIdentifiable
    {
        string Name { get; }

        IBody Body { get; }
        IOrbit Orbit { get; }

        double RocketVerticalHeight { get; }

        // maneuvers
        List<IManeuver> Maneuvers { get; }
        IManeuver NextManeuver { get; }
        void AddApoapsisManeuver(double targetRadius);
        void AddPeriapsisManeuver(double targetRadius);
        void AddHohmannManeuver(IBody targetBody);
        void AddInclinationChangeManeuver(IBody targetBody);

        // interaction
        double Throttle { get; set; }
        void LockHeading(IEulerAngles3 newHeading);
        void SetCompassSteering(double pitchAboveHorizon, double degreesFromNorth, double roll);
        void SetSteering(IVector3 forward);
        void Stage();
        void RaiseGear();
        void LowerGear();

        // status
        double MissionTime { get; }
        bool Landed { get; }
        bool InCircularOrbit { get; }

        double Altitude { get; }
        double AltitudeOverGround { get; }

        // orientation
        bool Turned { get; }
        double TurnDeviation { get; }
        double Heading { get; }
        double Pitch { get; }
        double Roll { get; }
        IVector3 Up { get; }
        IVector3 OrbitRetrograde { get; }
        IVector3 SurfaceRetrograde { get; }

        // thrust
        double MaxAcceleration { get; }
        double CurrentThrust { get; }
        double MaxThrust { get; }

        // speed
        double VerticalSpeed { get; }
        double HorizontalSurfaceSpeed { get; }
        IVector3 SurfaceVelocity { get; }

        double TimeToMissionTime(double pointInTime);
    }
}
