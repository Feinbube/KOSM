using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Game;

namespace KOSM.States
{
    public class OnGroundState : State
    {
        public OnGroundState(World world, string bodyName) : base(world, bodyName) { }

        public override string ToString()
        {
            return Body.Name;
        }
    }
}
