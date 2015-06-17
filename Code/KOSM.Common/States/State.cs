using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;

namespace KOSM.Common
{
    public abstract class State : IState
    {
        public IBody Body { get; private set; }

        public State(IWorld world, string bodyName)
        {
            this.Body = world.FindBodyByName(bodyName);
        }
    }
}
