using ABus.Config.MessageEndpoint;
using ABus.Config.Pipeline;
using ABus.Config.Transactions;
using ABus.Config.Transport;
using ABus.Contracts;
using Microsoft.Practices.ServiceLocation;

namespace ABus.Config
{
    public class ConfigurationGrammar
    {
        internal Configuration Configuration = new Configuration();

        public ConfigurationGrammar()
        {
            this.Pipeline = new PipelineConfigurationGrammar(this);
        }

        public PipelineConfigurationGrammar Pipeline { get; private set; }

        public TransportDefinitionGrammar UseTransport<T>(string name)
        {
            var definition = new TransportDefinition { TransportType = typeof(T).AssemblyQualifiedName, Name = name };
            this.Configuration.AvailableTransports.Add(definition);

            return new TransportDefinitionGrammar(this, definition);
        }

        public MessageEndpointGrammar WithMessageEndpoint { get { return new MessageEndpointGrammar(this);} }

        public TransactionsGrammar Transactions { get { return new TransactionsGrammar(this);} }

        public ConfigurationGrammar EnsureQueueExists()
        {
            this.Configuration.EnsureQueuesExist = true;
            return this;
        }

        public ConfigurationGrammar UseContainer(IServiceLocator container)
        {
            this.Configuration.Container = container;

            return this;
        }
    }
}