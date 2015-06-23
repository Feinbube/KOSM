using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KOSM.Interfaces
{
    public interface IPointInOrbit
    {
        double Altitude { get; }
        double Radius { get; }
        double TimeTill { get; }
        double TimeOf { get; }
        bool MovingTowards { get; }
        double Velocity { get; }
    }
}
