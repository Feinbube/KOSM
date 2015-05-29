using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Game;

namespace KOSM.Tasks
{
    public class CircularizeOrbitTask : RocketTask
    {
        public CircularizeOrbitTask(World world, Rocket rocket) : base(world, rocket) { }

        public override void Execute(World world, Mission mission)
        {
            throw new NotImplementedException();
        }

        public override string InfoText
        {
            get { return "Circulizing the orbit."; }
        }
    }
}
