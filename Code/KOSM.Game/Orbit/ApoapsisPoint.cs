using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;

namespace KOSM.Game
{
    public class ApoapsisPoint : PointInOrbit
    {
        public ApoapsisPoint(World world, Orbit orbit) : base(world, orbit)
        {
        }

        public override double Altitude
        {
            get { return orbit.raw.ApA; }
        }

        public override double Radius
        {
            get { return orbit.raw.ApR; }
        }

        public override double TimeTill
        {
            get { return orbit.raw.timeToAp; }
        }

        public override bool MovingTowards
        {
            get { return orbit.raw.timeToAp < orbit.raw.timeToPe; }
        }
    }
}
