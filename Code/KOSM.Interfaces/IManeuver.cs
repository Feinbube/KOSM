using System;

namespace KOSM.Interfaces
{
    public interface IManeuver
    {
        bool Completed { get; }
        void Complete();
        void Abort();

        IVectorXYZ DeltaV { get; }
        IVectorXYZ BurnVector { get; }

        double BurnDuration { get; }
        
        double TimeWhenDue { get; }
        double TimeOfBurn { get; }
        double TimeOfTurn { get; }

        double TimeTillDue { get; }
        double TimeTillBurn { get; }        
        double TimeTillTurn { get; }
    }
}
