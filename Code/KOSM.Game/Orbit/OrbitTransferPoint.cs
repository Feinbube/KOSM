using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;

namespace KOSM.Game
{
    public abstract class OrbitTransferPoint : PointInOrbit
    {
        protected Orbit targetOrbit = null;

        public OrbitTransferPoint(World world, Orbit orbit, Orbit targetOrbit) : base(world, orbit)
        {
            this.targetOrbit = targetOrbit;
        }
    }
}
