using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Game;

namespace KOSM.States
{
    public abstract class State
    {
        public Body Body { get; private set; }

        public State(World world, string bodyName)
        {
            this.Body = world.FindBodyByName(bodyName);
        }
    }
}
