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
    }

}