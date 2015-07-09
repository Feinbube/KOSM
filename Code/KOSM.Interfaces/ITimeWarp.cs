using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KOSM.Interfaces
{
    public interface ITimeWarp
    {
        bool IsTimeWarping { get; }
        bool WarpTime(double timespan);
        bool WarpTimeTo(double timeToWarpTo);
        void PreventTimeWarping();
    }
}
