using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Game;

namespace KOSM.Tasks
{
    public class HohmannTransferTask : RocketTask
    {
        protected Body body = null;

        public HohmannTransferTask(World world, Rocket rocket, Body body)
            : base(world, rocket)
        {
            this.body = body;
        }

        public override void Execute(World world, Mission mission)
        {
            throw new NotImplementedException();
        }

        public override string InfoText
        {
            get { return "Executing a Hohmann transfer to " + body.Name + "."; }
        }
    }
}
