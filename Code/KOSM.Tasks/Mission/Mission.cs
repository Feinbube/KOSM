using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KOSM.Interfaces;

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

        public void PushAtEnd(Task task)
        {
            this.tasks.Add(task);
        }

        public void PushBefore(Task parent, Task task)
        {
            this.tasks.Insert(tasks.IndexOf(parent), task);
        }

        public void PushBefore(Task parent, params Task[] tasks)
        {
            this.tasks.InsertRange(this.tasks.IndexOf(parent), tasks);
        }

        public void PushAfter(Task parent, Task task)
        {
            this.tasks.Insert(tasks.IndexOf(parent)+1, task);
        }

        public void PushAfter(Task parent, params Task[] tasks)
        {
            this.tasks.InsertRange(this.tasks.IndexOf(parent)+1, tasks);
        }

        public void Complete(IWorld world, Task task)
        {
            UpdateLog(world, task);
            this.tasks.Remove(task);
        }

        public void Abort(IWorld world, Task task)
        {
            UpdateLog(world, task);
            this.tasks.Remove(task);
        }

        public void Execute(IWorld world)
        {
            if (this.IsComplete)
                return;

            UpdateLog(world, this.tasks[0]);
            this.tasks[0].Execute(world, this);

            ShowMissionPlan(world);
        }

        public void ShowMissionPlan(IWorld world)
        {
            world.MissionPlanLog.Clear();
            for (int i = 0; i < tasks.Count; i++)
                world.MissionPlanLog.Add(tasks[i].Description);
        }

        string latestTaskDescription = null;
        string latestTaskDetails = null;

        private void UpdateLog(IWorld world, Task currentTask)
        {
            if (currentTask.Description != latestTaskDescription)
                world.MissionLog.Add(currentTask.Description);
            latestTaskDescription = currentTask.Description;

            if (currentTask.Details != null && currentTask.Details != latestTaskDetails)
                world.MissionLog.Add("  " + currentTask.Details);
            latestTaskDetails = currentTask.Details;
        }
    }
}
