using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Game;

namespace KOSM.States
{
    public class InOrbitState : State
    {
        public InOrbitState(World world, string bodyName) : base(world, bodyName) { }

        public override string ToString()
        {
            return "an orbit around " + Body.Name;
        }
    }
}
