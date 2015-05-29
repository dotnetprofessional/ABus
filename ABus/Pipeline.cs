using System;
using System.Collections.Generic;
using ABus.Contracts;
using ABus.Tasks.Inbound;
using ABus.Tasks.Outbound;
using ABus.Tasks.Startup;
using Microsoft.Practices.ServiceLocation;

namespace ABus
{
    public class InboundMessageStages
    {
        public const string Security = "Security";
        public const string TransactionManagement = "TransactionManagement";
        public const string TransformInboundRawMessage = "TransformInboundRawMessage";
        public const string Deserialize = "Deserialize";
        public const string TransformInboundMessage = "TransformInboundMessage";
        public const string ExecuteHandler = "ExecuteHandler";
    }

    public class OutboundMessageStages
    {
        public const string ValidateBestPractices = "ValidateBestPractices";
        public const string CreateRawMessage = "TransformOutboundMessage";
        public const string TransformOutboundMessage = "CreateRawMessage";
        public const string Serialize = "Serialize";
        public const string TransformOutboundRawMessage = "TransformOutboundRawMessage";
        public const string SendMessage = "SendMessage";
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
        ABusTraceSource Trace { get; set; }
        PipelineContext PipelineContext { get; set; }

        //BlockingCollection<IPipelineTask> Tasks;

        public Pipeline(IServiceLocator serviceLocator)
        {
            this.ServiceLocator = serviceLocator;
            this.StartupPipelineTasks = new PipelineTasks();
            this.InboundMessagePipelineTasks = new PipelineTasks();
            this.OutboundMessagePipelineTasks = new PipelineTasks();
            this.Trace = new ABusTraceSource();

            this.StartupPipeline= new StartupPipelineGrammer(this, "Startup");
            this.InboundMessagePipeline = new InboundMessagePipelineGrammer(this, "InboundMessage");
            this.OutboundMessagePipeline = new OutboundMessagePipelineGrammer(this, "OutboundMessage");

            //this.Tasks = new BlockingCollection<IPipelineTask>();

            // Register the known startup stages
            this.StartupPipelineTasks.AddStage(StartupStages.Initialize);

            // Register the known inbound message stages
            this.InboundMessagePipelineTasks.AddStage(InboundMessageStages.Security);
            this.InboundMessagePipelineTasks.AddStage(InboundMessageStages.TransactionManagement);
            this.InboundMessagePipelineTasks.AddStage(InboundMessageStages.TransformInboundRawMessage);
            this.InboundMessagePipelineTasks.AddStage(InboundMessageStages.Deserialize);
            this.InboundMessagePipelineTasks.AddStage(InboundMessageStages.TransformInboundMessage);
            this.InboundMessagePipelineTasks.AddStage(InboundMessageStages.ExecuteHandler);

            this.OutboundMessagePipelineTasks.AddStage(OutboundMessageStages.ValidateBestPractices);
            this.OutboundMessagePipelineTasks.AddStage(OutboundMessageStages.TransformOutboundMessage);
            this.OutboundMessagePipelineTasks.AddStage(OutboundMessageStages.CreateRawMessage);
            this.OutboundMessagePipelineTasks.AddStage(OutboundMessageStages.Serialize);
            this.OutboundMessagePipelineTasks.AddStage(OutboundMessageStages.TransformOutboundRawMessage);
            this.OutboundMessagePipelineTasks.AddStage(OutboundMessageStages.SendMessage);

            // Register the known startup stage tasks
            this.StartupPipeline.Initialize.Register("TransportDefinitions", typeof (DefineTransportDefinitionsTask))
                .Then("ScanMessageTypes", typeof(ScanMessageTypesTask))
                .Then("ScanHandlers", typeof(ScanMessageHandlersTask))
                .Then("AssignTransportToMessageTypes", typeof(AssignTransportToMessageTypesTask))
                .Then("InitializeTransports", typeof(InitializeTransportsTask))
                .Then("ValidateQueues", typeof(ValidateQueuesTask))
                .Then("InitializeHandlers", typeof(InitializeHandlersTask))
                
                .Then("Task2", typeof(InitailizePipeline2));

            // Inbound Message Tasks
            this.InboundMessagePipeline.TransactionManagement
                .Register("ExceptionHander", typeof(ExceptionHanderTask))
                .Then("EnableTransactionManagement", typeof (EnableTransactionManagementTask));

            this.InboundMessagePipeline.Deserialize
                .Register("DeserializeMessage", typeof(DeserializeMessageFromJsonTask));

            this.InboundMessagePipeline.ExecuteHandler
                .Register("InvokeHandler", typeof(InvokeHandlerTask));


            // Outbound Message Tasks
            
            this.OutboundMessagePipeline.CreateRawMessage
                .Register("CreateOutboundMessage", typeof(CreateOutboundMessageTask))
                .Register("AppendCommonMetaData", typeof (AppendCommonMetaDataTask));

            this.OutboundMessagePipeline.Serialize
                .Register("SerializeMessage", typeof(SerializeMessageToJsonTask));

            this.OutboundMessagePipeline.SendMessage
                .Register("SendMessage", typeof (SendMessageTask));

        }

        /// <summary>
        /// Start processing the pipeline
        /// </summary>
        public void Start()
        {
            this.PipelineContext = new PipelineContext(this.ServiceLocator, this.Trace);
            this.PipelineContext.MessageReceivedHandler += InboundMessageReceived;

            var tasks = this.StartupPipelineTasks.GetTasks();
            if(tasks.Count > 0)
                this.ExecuteStartupTask(this.PipelineContext, tasks.First);
        }

        void InboundMessageReceived(object sender, RawMessage e)
        {
            // Initialize the message context
            var inboundMessageContext = new InboundMessageContext(sender as string, e, this.PipelineContext);
            var bus = new Bus(inboundMessageContext, this);
            inboundMessageContext.Bus = bus;

            // Start the inbound message pipeline
            var tasks = this.InboundMessagePipelineTasks.GetTasks();
            if (tasks.Count > 0)
                this.ExecuteInboundMessageTask(inboundMessageContext, tasks.First);
        }

        public void SendOutboundMessage(InboundMessageContext inboundMessageContext, OutboundMessageContext.MessageIntent messageIntent, object messageInstance)
        {
            // Initialize the message context
            var registeredType = this.PipelineContext.RegisteredMessageTypes[messageInstance.GetType().FullName];

            var outBoundMessageContext = new OutboundMessageContext(new QueueEndpoint { Host = registeredType.Transport.Uri, Name = registeredType.Queue }, messageInstance, this.PipelineContext, inboundMessageContext);
            // Record the messageIntent type
            outBoundMessageContext.RawMessage.MetaData.Add(new MetaData { Name = StandardMetaData.MessageIntent, Value = messageIntent.ToString() });

            // Start the inbound message pipeline
            var tasks = this.OutboundMessagePipelineTasks.GetTasks();
            if (tasks.Count > 0)
                this.ExecuteOutboundMessageTask(outBoundMessageContext, tasks.First);
        }

        public StartupPipelineGrammer StartupPipeline { get; private set; } 
        public InboundMessagePipelineGrammer InboundMessagePipeline { get; private set; }

        public OutboundMessagePipelineGrammer OutboundMessagePipeline { get; private set; }

        Pipeline RegisterStartupTask(string stage, PipelineTask task)
        {
            if (!this.StartupPipelineTasks.StageExists(stage))
                throw new ArgumentException("There is no stage named: " + stage);

            this.StartupPipelineTasks.AddTask(stage, task);
            return this;
        }

        Pipeline RegisterInboundMessageTask(string stage, PipelineTask task)
        {
            if (!this.InboundMessagePipelineTasks.StageExists(stage))
                throw new ArgumentException("There is no stage named: " + stage);

            this.InboundMessagePipelineTasks.AddTask(stage, task);
            return this;
        }

        Pipeline RegisterOutboundMessageTask(string stage, PipelineTask task)
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

        void ExecuteInboundMessageTask(InboundMessageContext context, LinkedListNode<PipelineTask> task)
        {

            var taskInstance = this.ServiceLocator.GetInstance(task.Value.Task) as IPipelineInboundMessageTask;
            taskInstance.Invoke(context, () =>
            { 
                if (task.Next != null)
                    this.ExecuteInboundMessageTask(context, task.Next);
            });
        }
        void ExecuteOutboundMessageTask(OutboundMessageContext context, LinkedListNode<PipelineTask> task)
        {

            var taskInstance = this.ServiceLocator.GetInstance(task.Value.Task) as IPipelineOutboundMessageTask;
            taskInstance.Invoke(context, () =>
            {
                if (task.Next != null) 
                    this.ExecuteOutboundMessageTask(context, task.Next);
            });
        }
    }

    public class InboundMessagePipelineGrammer : PipelineGrammar
    {
        
        public PipelineStageGrammar Security
        {
            get { return new PipelineStageGrammar(this, InboundMessageStages.Security); }
        }

        public PipelineStageGrammar TransactionManagement
        {
            get { return new PipelineStageGrammar(this, InboundMessageStages.TransactionManagement); }
        }

        public PipelineStageGrammar TransformInboundRawMessage
        {
            get { return new PipelineStageGrammar(this, InboundMessageStages.TransformInboundRawMessage); }
        }

        public PipelineStageGrammar Deserialize
        {
            get { return new PipelineStageGrammar(this, InboundMessageStages.Deserialize); }
        }

        public PipelineStageGrammar TransformInboundMessage
        {
            get { return new PipelineStageGrammar(this, InboundMessageStages.TransformInboundMessage); }
        }

        public PipelineStageGrammar ExecuteHandler
        {
            get { return new PipelineStageGrammar(this, InboundMessageStages.ExecuteHandler); }
        }

        public InboundMessagePipelineGrammer(Pipeline associatedPipeline, string pipelineName) : base(associatedPipeline, pipelineName)
        {
        }
    }

    public class OutboundMessagePipelineGrammer : PipelineGrammar
    {
        public PipelineStageGrammar ValidateBestPractices
        {
            get { return new PipelineStageGrammar(this, OutboundMessageStages.ValidateBestPractices); }
        }
        
        public PipelineStageGrammar TransformInboundRawMessage
        {
            get { return new PipelineStageGrammar(this, OutboundMessageStages.TransformOutboundRawMessage); }
        }

        public PipelineStageGrammar CreateRawMessage
        {
            get { return new PipelineStageGrammar(this, OutboundMessageStages.CreateRawMessage); }
        }

        public PipelineStageGrammar Serialize
        {
            get { return new PipelineStageGrammar(this, OutboundMessageStages.Serialize); }
        }

        public PipelineStageGrammar TransformInboundMessage
        {
            get { return new PipelineStageGrammar(this, OutboundMessageStages.TransformOutboundMessage); }
        }


        public PipelineStageGrammar SendMessage
        {
            get { return new PipelineStageGrammar(this, OutboundMessageStages.SendMessage); }
        }

        public OutboundMessagePipelineGrammer(Pipeline associatedPipeline, string pipelineName)
            : base(associatedPipeline, pipelineName)
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
