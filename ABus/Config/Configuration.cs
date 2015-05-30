using System.Collections.Generic;
using ABus.Config.Pipeline;
using ABus.Config.Transport;
using ABus.Contracts;

namespace ABus.Config
{
    public class Configuration
    {
        public Configuration()
        {
            this.Pipeline = new PipelineConfiguration();
            this.AvailableTransports = new List<TransportDefinition>();
        }

        public PipelineConfiguration Pipeline { get; private set; }

        public List<TransportDefinition> AvailableTransports { get; set; }
 
        public bool EnsureQueuesExist { get; set; }
    }

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

        public Configuration EnsureQueueExists()
        {
            this.Configuration.EnsureQueuesExist = true;
            return this.Configuration;
        }
    }
}
