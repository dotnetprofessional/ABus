using System;
using ABus.Tasks.Inbound;
using ABus.Tasks.Outbound;
using ABus.Tasks.Startup;

namespace ABus.Config.Pipeline
{
    public class PipelineConfigurationGrammar
    {
        ConfigurationGrammar Parent { get; set; }
        PipelineConfiguration Config { get; set; }

        public PipelineConfigurationGrammar(ConfigurationGrammar parent)
        {
            this.Parent = parent;
            this.Config = parent.Configuration.Pipeline;
            this.Startup = new StartupPipelineGrammer(this.Parent, "Startup");
            this.InboundMessage = new InboundMessagePipelineGrammer(this.Parent, "InboundMessage");
            this.OutboundMessage = new OutboundMessagePipelineGrammer(this.Parent, "OutboundMessage");

            // Register the known startup stage tasks
            this.Startup.Initialize.Register<DefineTransportDefinitionsTask>()
                .Then<ScanMessageTypesTask>()
                .Then<ScanMessageHandlersTask>()
                .Then<AssignTransportToMessageTypesTask>()
                .Then<InitializeTransportsTask>()
                .Then<ValidateQueuesTask>()
                .Then<InitializeHandlersTask>()

                .Then<InitailizePipeline2>();

            // Inbound Message Tasks
            this.InboundMessage.TransactionManagement
                .Register<ExceptionHanderTask>()
                .Then<EnableTransactionManagementTask>();

            this.InboundMessage.Deserialize
                .Register<DeserializeMessageFromJsonTask>();

            this.InboundMessage.ExecuteHandler
                .Register<InvokeHandlerTask>();

            // Outbound Message Tasks
            this.OutboundMessage.CreateRawMessage
                .Register<CreateOutboundMessageTask>()
                .Then<AppendCommonMetaDataTask>();

            this.OutboundMessage.Serialize
                .Register<SerializeMessageToJsonTask>();

            this.OutboundMessage.SendMessage
                .Register<SendMessageTask>();
        }

        #region public properties/methods

        public StartupPipelineGrammer Startup { get; private set; }
        public InboundMessagePipelineGrammer InboundMessage { get; private set; }

        public OutboundMessagePipelineGrammer OutboundMessage { get; private set; }

        #endregion 
    }

    public class PipelineConfiguration
    {
        internal PipelineTasks StartupPipelineTasks;
        internal PipelineTasks InboundMessagePipelineTasks;
        internal PipelineTasks OutboundMessagePipelineTasks;

        public PipelineConfiguration()
        {
            this.StartupPipelineTasks = new PipelineTasks();
            this.InboundMessagePipelineTasks = new PipelineTasks();
            this.OutboundMessagePipelineTasks = new PipelineTasks();

            this.InitializePipelineConfiguration();
        }

        void InitializePipelineConfiguration()
        {
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

           
        }

        PipelineConfiguration RegisterStartupTask(string stage, PipelineTask task)
        {
            if (!this.StartupPipelineTasks.StageExists(stage))
                throw new ArgumentException("There is no stage named: " + stage);

            this.StartupPipelineTasks.AddTask(stage, task);
            return this;
        }

        PipelineConfiguration RegisterInboundMessageTask(string stage, PipelineTask task)
        {
            if (!this.InboundMessagePipelineTasks.StageExists(stage))
                throw new ArgumentException("There is no stage named: " + stage);

            this.InboundMessagePipelineTasks.AddTask(stage, task);
            return this;
        }

        PipelineConfiguration RegisterOutboundMessageTask(string stage, PipelineTask task)
        {
            if (!this.OutboundMessagePipelineTasks.StageExists(stage))
                throw new ArgumentException("There is no stage named: " + stage);

            this.OutboundMessagePipelineTasks.AddTask(stage, task);
            return this;
        }

        internal PipelineConfiguration Register(string pipeline, string stage, PipelineTask task)
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
    }
}