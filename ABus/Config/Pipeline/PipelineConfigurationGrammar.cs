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
}