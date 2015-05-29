using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Game;

namespace KOSM.Tasks
{
    public abstract class Task
    {
        public abstract void Execute(World world, Mission mission);

        public abstract string InfoText { get; }
    }
}
