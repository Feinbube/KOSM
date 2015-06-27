using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;
using KOSM.Common;

namespace KOSM.Tasks
{
    public class TestTask : RocketTask
    {
        public TestTask(IWorld world, IRocket rocket)
            : base(world, rocket)
        {
        }
        
        public override void Execute(IWorld world, Mission mission)
        {
            rocket.SetSteering(rocket.OrbitRetrograde);
        }

        public override string Description
        {
            get { return "Testing."; }
        }
    }
}
