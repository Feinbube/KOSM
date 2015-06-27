﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;

namespace KOSM.Game
{
    public class ApoapsisPoint : PointInOrbit
    {
        public ApoapsisPoint(World world, Orbit orbit)
            : base(world, orbit)
        {
        }

        // TODO: for efficiency override radius, altitude, timeto, ...

        protected override double degreesFromPeriapsis
        {
            get { return 180; }
        }
    }
}
