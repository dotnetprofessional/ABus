using System;
using System.Collections.Generic;

namespace ABus
{
    public class PipelineTasks 
    {
        public PipelineTasks()
        {
            this.Stages = new StageCollection();
        }

        StageCollection Stages { get; set; } 

        /// <summary>
        /// Returns an ordered list of tasks
        /// </summary>
        /// <returns></returns>
        public LinkedList<Type> GetTasks()
        {
            LinkedList<Type> tasks = null;

            for (int i = 0; i < this.Stages.Count; i++)
            {
                if (tasks == null && this.Stages[i].Tasks.Count > 0)
                    tasks = this.Stages[i].Tasks;
                else if (this.Stages[i].Tasks.Count > 0)
                    tasks.AddLast(this.Stages[i].Tasks.First);
            }

            return tasks;
        }

        public bool StageExists(string name)
        {
            return this.Stages.Contains(name);
        }

        public void AddStage(string name)
        {
            this.Stages.Add(new Stage(name));
        }

        public void AddTask(string stageName, Type task)
        {
            var stage = this.Stages[stageName];
            stage.Tasks.AddLast(task);
        }



    }
}