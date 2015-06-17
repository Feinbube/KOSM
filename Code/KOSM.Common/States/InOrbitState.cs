using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;

namespace KOSM.Common
{
    public class InOrbitState : State, IInOrbitState
    {
        public InOrbitState(IWorld world, string bodyName) : base(world, bodyName) { }

        public override string ToString()
        {
            return "an orbit around " + Body.Name;
        }
    }
}
