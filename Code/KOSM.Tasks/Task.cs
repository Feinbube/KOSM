using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;

namespace KOSM.Tasks
{
    public abstract class Task
    {
        public abstract void Execute(IWorld world, Mission mission);

        public abstract string Description { get; }

        public string Details { get; set; }
    }
}
