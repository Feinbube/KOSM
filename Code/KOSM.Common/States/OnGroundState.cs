using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;

namespace KOSM.Common
{
    public class OnGroundState : State, IOnGroundState
    {
        public OnGroundState(IWorld world, string bodyName) : base(world, bodyName) { }

        public override string ToString()
        {
            return "the ground of " + Body.Name;
        }
    }
}
