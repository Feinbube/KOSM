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
        double Velocity { get; }
        
        bool MovingTowards { get; }
        double DegreesTo { get; }
        double DegreesFrom { get; }
        
        double TimeTill { get; }
        double TimeOf { get; }
        double TimeTillDegreesToEquals(double degreesTo);
        double TimeTillDegreesFromEquals(double degreesFrom);
    }
}
