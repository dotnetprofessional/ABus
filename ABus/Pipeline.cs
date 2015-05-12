using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity.Configuration;

namespace ABus
{
    public class Pipeline
    {
        IServiceLocator ServiceLocator { get; set; }

        public class PiplineStages
        {
            public const string Initialize = "Initialize";
            public const string Authentication = "Authentication";
            public const string Authorize = "Authorize";
            public const string Deserialize = "Deserialize";
            public const string MapHandler = "MapHandler";
            public const string ExecuteHandler = "ExecuteHandler";
            public const string PostHandlerExecution = "PostHandlerExecution";

        }

        PipelineTasks PipelineTasks;
        PipelineTasks InitializationPipeline;
        //BlockingCollection<IPipelineTask> Tasks;

        public Pipeline(IServiceLocator serviceLocator)
        {
            this.ServiceLocator = serviceLocator;
            this.PipelineTasks = new PipelineTasks();

            this.InitializationPipeline = new PipelineTasks();
            this.InitializationPipeline.AddStage(PiplineStages.Initialize);

            //this.Tasks = new BlockingCollection<IPipelineTask>();

            // Register the known stages
            this.PipelineTasks.AddStage(PiplineStages.Authentication);
            this.PipelineTasks.AddStage(PiplineStages.Authorize);
            this.PipelineTasks.AddStage(PiplineStages.Deserialize);
            this.PipelineTasks.AddStage(PiplineStages.MapHandler);
            this.PipelineTasks.AddStage(PiplineStages.ExecuteHandler);
            this.PipelineTasks.AddStage(PiplineStages.PostHandlerExecution);

            // Define system tasks and put them in the appropriate pipeline stages
            this.InitializationPipeline.AddTask(PiplineStages.Initialize, typeof(InitailizePipeline));
            this.InitializationPipeline.AddTask(PiplineStages.Initialize, typeof(InitailizePipeline2));
        }

        /// <summary>
        /// Start processing the pipeline
        /// </summary>
        public void Start()
        {
            var tasks = this.InitializationPipeline.GetTasks();
            if(tasks.Count > 0)
                this.ExecuteInitializationTask(new PipelineContext(), tasks.First);
        }

        internal Pipeline Register<T>(string name)
        {
            if (!this.PipelineTasks.StageExists(name))
                throw new ArgumentException("There is no stage named: " + name);

            this.PipelineTasks.AddTask(name, typeof (T));
            return this;
        }

        void ExecuteInitializationTask(PipelineContext context, LinkedListNode<Type> task)
        {

            var taskInstance = this.ServiceLocator.GetInstance(task.Value) as IPipelineInitializationTask;
            taskInstance.Invoke(context, () =>
            {
                if (task.Next != null)
                    this.ExecuteInitializationTask(context, task.Next);
            });
        }

        void ExecuteMessageTask(MessageContext context, LinkedListNode<Type> task)
        {

            var taskInstance = this.ServiceLocator.GetInstance(task.Value) as IPipelineMessageTask;
            taskInstance.Invoke(context, () => this.ExecuteMessageTask(context, task.Next));
        }


        public PipelineStageGrammar Authenticate
        {
            get { return new PipelineStageGrammar(this, PiplineStages.Authentication); }
        }

        public PipelineStageGrammar Authorize
        {
            get { return new PipelineStageGrammar(this, PiplineStages.Authorize); }
        }

        public PipelineStageGrammar Deserialize
        {
            get { return new PipelineStageGrammar(this, PiplineStages.Deserialize); }
        }

        public PipelineStageGrammar MapHandler
        {
            get { return new PipelineStageGrammar(this, PiplineStages.MapHandler); }
        }

        public PipelineStageGrammar ExecuteHandler
        {
            get { return new PipelineStageGrammar(this, PiplineStages.ExecuteHandler); }
        }

        public PipelineStageGrammar PostHandlerExecution
        {
            get { return new PipelineStageGrammar(this, PiplineStages.PostHandlerExecution); }
        }

    }
}
