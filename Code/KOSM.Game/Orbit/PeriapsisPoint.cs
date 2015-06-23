using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;

namespace KOSM.Game
{
    public class PeriapsisPoint : PointInOrbit
    {
        public PeriapsisPoint(World world, Orbit orbit)
            : base(world, orbit)
        {
        }
        public override double Altitude
        {
            get { return orbit.raw.PeA; }
        }

        public override double Radius
        {
            get { return orbit.raw.PeR; }
        }

        public override double TimeTill
        {
            get { return orbit.raw.timeToPe; }
        }

        public override bool MovingTowards
        {
            get { return orbit.raw.timeToPe < orbit.raw.timeToAp; }
        }        
    }
}
