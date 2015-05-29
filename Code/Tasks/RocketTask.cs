using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Game;

namespace KOSM.Tasks
{
    public abstract class RocketTask : Task
    {
        protected Rocket rocket = null;

        public RocketTask(World world, Rocket rocket)
        {
            this.rocket = rocket;
        }
    }
}
