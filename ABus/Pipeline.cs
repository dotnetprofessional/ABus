using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ABus.Tasks;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity.Configuration;

namespace ABus
{
    public class InboundMessageStages
    {
        public const string Authentication = "Authentication";
        public const string Authorize = "Authorize";
        public const string Deserialize = "Deserialize";
        public const string MapHandler = "MapHandler";
        public const string ExecuteHandler = "ExecuteHandler";
        public const string PostHandlerExecution = "PostHandlerExecution";
    }

    public class StartupStages
    {
        public const string Initialize = "Initialize";
    }
    public class Pipeline
    {
        IServiceLocator ServiceLocator { get; set; }

        PipelineTasks StartupPipelineTasks;
        PipelineTasks InboundMessagePipelineTasks;
        PipelineTasks OutboundMessagePipelineTasks;

        //BlockingCollection<IPipelineTask> Tasks;

        public Pipeline(IServiceLocator serviceLocator)
        {
            this.ServiceLocator = serviceLocator;
            this.StartupPipelineTasks = new PipelineTasks();
            this.InboundMessagePipelineTasks = new PipelineTasks();
            this.OutboundMessagePipelineTasks = new PipelineTasks();

            this.StartupPipeline= new StartupPipelineGrammer(this, "Startup");
            this.InboundMessagePipeline = new InboundMessagePipelineGrammer(this, "InboundMessage");

            //this.Tasks = new BlockingCollection<IPipelineTask>();

            // Register the known startup stages
            this.StartupPipelineTasks.AddStage(StartupStages.Initialize);

            // Register the known inbound message stages
            this.InboundMessagePipelineTasks.AddStage(InboundMessageStages.Authentication);
            this.InboundMessagePipelineTasks.AddStage(InboundMessageStages.Authorize);
            this.InboundMessagePipelineTasks.AddStage(InboundMessageStages.Deserialize);
            this.InboundMessagePipelineTasks.AddStage(InboundMessageStages.MapHandler);
            this.InboundMessagePipelineTasks.AddStage(InboundMessageStages.ExecuteHandler);
            this.InboundMessagePipelineTasks.AddStage(InboundMessageStages.PostHandlerExecution);

            // Register the known startup stage tasks
            this.StartupPipeline.Initialize.Register("TransportDefinitions", typeof (DefineTransportDefinitionsTask))
                .Then("ScanMessageTypes", typeof(ScanMessageTypesTask))
                .Then("ScanHandlers", typeof(ScanMessageHandlersTask))
                .Then("AssignTransportToMessageTypes", typeof(AssignTransportToMessageTypesTask))
                .Then("InitializeTransports", typeof(InitializeTransportsTask))
                .Then("ValidateQueues", typeof(ValidateQueuesTask))
                .Then("Task2", typeof(InitailizePipeline2));
        }

        /// <summary>
        /// Start processing the pipeline
        /// </summary>
        public void Start() 
        {
            var tasks = this.StartupPipelineTasks.GetTasks();
            if(tasks.Count > 0)
                this.ExecuteStartupTask(new PipelineContext(this.ServiceLocator), tasks.First);
        }


        public StartupPipelineGrammer StartupPipeline { get; private set; }
        public InboundMessagePipelineGrammer InboundMessagePipeline { get; private set; }

        internal Pipeline RegisterStartupTask(string stage, PipelineTask task)
        {
            if (!this.StartupPipelineTasks.StageExists(stage))
                throw new ArgumentException("There is no stage named: " + stage);

            this.StartupPipelineTasks.AddTask(stage, task);
            return this;
        }
        internal Pipeline RegisterInboundMessageTask(string stage, PipelineTask task)
        {
            if (!this.InboundMessagePipelineTasks.StageExists(stage))
                throw new ArgumentException("There is no stage named: " + stage);

            this.InboundMessagePipelineTasks.AddTask(stage, task);
            return this;
        }

        internal Pipeline RegisterOutboundMessageTask(string stage, PipelineTask task)
        {
            if (!this.OutboundMessagePipelineTasks.StageExists(stage))
                throw new ArgumentException("There is no stage named: " + stage);

            this.OutboundMessagePipelineTasks.AddTask(stage, task);
            return this;
        }

        internal Pipeline Register(string pipeline, string stage, PipelineTask task)
        {
            switch (pipeline)
            {
                case "Startup":
                    return this.RegisterStartupTask(stage, task);
                case "InboundMessage":
                    return this.RegisterInboundMessageTask(stage, task);
                case "OutboundMessage":
                    return this.RegisterOutboundMessageTask(stage, task);
                default:
                    throw new ArgumentException("Unknown pipeline " + pipeline);
            }   
        }
        void ExecuteStartupTask(PipelineContext context, LinkedListNode<PipelineTask> task)
        {

            var taskInstance = this.ServiceLocator.GetInstance(task.Value.Task) as IPipelineStartupTask;
            taskInstance.Invoke(context, () =>
            {
                if (task.Next != null)
                    this.ExecuteStartupTask(context, task.Next);
            });
        }

        void ExecuteMessageTask(MessageContext context, LinkedListNode<PipelineTask> task)
        {

            var taskInstance = this.ServiceLocator.GetInstance(task.Value.Task) as IPipelineMessageTask;
            taskInstance.Invoke(context, () => this.ExecuteMessageTask(context, task.Next));
        }
    }

    public class InboundMessagePipelineGrammer : PipelineGrammar
    {
        
        public PipelineStageGrammar Authenticate
        {
            get { return new PipelineStageGrammar(this, InboundMessageStages.Authentication); }
        }

        public PipelineStageGrammar Authorize
        {
            get { return new PipelineStageGrammar(this, InboundMessageStages.Authorize); }
        }

        public PipelineStageGrammar Deserialize
        {
            get { return new PipelineStageGrammar(this, InboundMessageStages.Deserialize); }
        }

        public PipelineStageGrammar MapHandler
        {
            get { return new PipelineStageGrammar(this, InboundMessageStages.MapHandler); }
        }

        public PipelineStageGrammar ExecuteHandler
        {
            get { return new PipelineStageGrammar(this, InboundMessageStages.ExecuteHandler); }
        }

        public PipelineStageGrammar PostHandlerExecution
        {
            get { return new PipelineStageGrammar(this, InboundMessageStages.PostHandlerExecution); }
        }

        public InboundMessagePipelineGrammer(Pipeline associatedPipeline, string pipelineName) : base(associatedPipeline, pipelineName)
        {
        }
    }

    public class StartupPipelineGrammer : PipelineGrammar
    {
        public PipelineStageGrammar Initialize
        {
            get { return new PipelineStageGrammar(this, StartupStages.Initialize); }
        }

        public StartupPipelineGrammer(Pipeline associatedPipeline, string pipelineName) : base(associatedPipeline, pipelineName)
        {
        }
    }
    public class PipelineGrammar
    {
        public PipelineGrammar(Pipeline associatedPipeline, string pipelineName)
        {
            AssociatedPipeline = associatedPipeline;
            PipelineName = pipelineName;
        }

        internal Pipeline AssociatedPipeline { get; set; }

        internal string PipelineName { get; set; }
    }
}
