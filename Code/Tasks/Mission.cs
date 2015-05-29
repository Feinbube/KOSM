using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Game;

namespace KOSM.Tasks
{
    public class Mission
    {
        private List<Task> tasks = new List<Task>();

        public bool IsComplete { get { return this.tasks.Count <= 0; } }

        public Mission() { }
        public Mission(params Task[] tasks)
        {
            this.tasks.AddRange(tasks);
        }

        public void Push(Task task)
        {
            this.tasks.Add(task);
        }

        public void PushAfter(Task parent, Task task)
        {
            this.tasks.Insert(tasks.IndexOf(parent), task);
        }

        public void PushAfter(Task parent, params Task[] tasks)
        {
            this.tasks.InsertRange(this.tasks.IndexOf(parent), tasks);
        }

        public void Complete(Task task)
        {
            this.tasks.Remove(task);
        }

        public void Execute(World world)
        {
            if (this.IsComplete)
                return;

            this.tasks[0].Execute(world, this);
        }
    }
}
