using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KOSM.Interfaces
{
    public interface IRocketControl
    {
        double Throttle { get; set; }
        void LockHeading(IEulerAngles3 newHeading);
        void SetCompassSteering(double pitchAboveHorizon, double degreesFromNorth, double roll);
        void SetSteering(IVectorXYZ forward);

        // orientation
        bool Turned { get; }
        double TurnDeviation { get; }
        double Heading { get; }
        double Pitch { get; }
        double Roll { get; }
        IVectorXYZ Up { get; }
        IVectorXYZ OrbitRetrograde { get; }
        IVectorXYZ SurfaceRetrograde { get; }

        void Stage();

        // thrust
        double MaxAcceleration { get; }
        double CurrentThrust { get; }
        double MaxThrust { get; }
    }
}
