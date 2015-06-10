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
        /// This represents a set of tasks for all stages that can't be modified once set.
        /// </summary>
        LinkedList<PipelineTask> MaterializedTasks { get; set; }
        /// <summary>
        /// Returns an ordered list of tasks
        /// </summary>
        /// <returns></returns>
        public LinkedList<PipelineTask> GetTasks()
        {
            if (this.MaterializedTasks == null)
            {
                LinkedList<PipelineTask> tasks = null;

                for (int i = 0; i < this.Stages.Count; i++)
                {
                    if (tasks == null && this.Stages[i].Tasks.Count > 0)
                        tasks = this.Stages[i].Tasks;
                    else if (this.Stages[i].Tasks.Count > 0)
                    {
                        // Need to transfer each of the nodes to the parent node
                        // While this might be 'slow' we're only dealing with a handful of nodes
                        var next = this.Stages[i].Tasks.First;
                        do 
                        {
                            tasks.AddLast(next.Value);
                            next = next.Next;
                        } while (next != null);
                    }
                }
                this.MaterializedTasks = tasks;
            }
            return this.MaterializedTasks;
        }

        public bool StageExists(string name)
        {
            return this.Stages.Contains(name);
        }

        public void AddStage(string name)
        {
            this.Stages.Add(new Stage(name));
        }

        public void AddTask(string stageName, PipelineTask task)
        {
            var stage = this.Stages[stageName];
            stage.Tasks.AddLast(task);
        }

        public void AddTaskBefore(string stageName, PipelineTask existingTask, PipelineTask task)
        {
            var stage = this.Stages[stageName];
            var existintNode = stage.Tasks.Find(existingTask);
            if (existintNode == null)
                throw new ArgumentException(string.Format("Unable to locate tyep {0} in stage {1}", existingTask.Name, stageName));

            stage.Tasks.AddBefore(existintNode, task);
        }

        public void AddTaskAfter(string stageName, PipelineTask existingTask, PipelineTask task)
        {
            var stage = this.Stages[stageName];
            var existintNode = stage.Tasks.Find(existingTask);
            if (existintNode == null)
                throw new ArgumentException(string.Format("Unable to locate tyep {0} in stage {1}", existingTask.Name, stageName));

            stage.Tasks.AddAfter(existintNode, task);
        }

        public void ReplaceTask(string stageName, PipelineTask existingTask, PipelineTask task)
        {
            var stage = this.Stages[stageName];
            var existintNode = stage.Tasks.Find(existingTask);
            if (existintNode == null)
                throw new ArgumentException(string.Format("Unable to locate tyep {0} in stage {1}", existingTask.Name, stageName));

            this.AddTaskBefore(stageName, existingTask, task);
            // Now remove the existing task
            stage.Tasks.Remove(existintNode);
        }
    }

}