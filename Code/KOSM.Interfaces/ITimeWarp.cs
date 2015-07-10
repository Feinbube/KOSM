using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KOSM.Interfaces
{
    public interface ITimeWarp
    {
        bool IsTimeWarping { get; }

        void PreventTimeWarping();

        bool OneTickWarpTimeBy(double factor);
        bool OneTickWarpTime(double timespan);
        bool OneTickWarpTimeTo(double timeToWarpTo);
        
        bool PersistentWarpTimeTo(double timeToWarpTo);
    }
}
