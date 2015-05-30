using System.Collections.Generic;
using ABus.Config.MessageEndpoint;
using ABus.Config.Pipeline;
using ABus.Config.Transactions;
using ABus.Contracts;

namespace ABus.Config
{
    public class Configuration
    {
        public Configuration()
        {
            this.Pipeline = new PipelineConfiguration();
            this.AvailableTransports = new List<TransportDefinition>();
            this.MessageEndpointDefinitions = new List<MessageEndpointDefinition>();
            this.Transactions = new TransactionOptions();
        }

        public PipelineConfiguration Pipeline { get; private set; }

        public List<TransportDefinition> AvailableTransports { get; private set; }

        public List<MessageEndpointDefinition> MessageEndpointDefinitions { get; private set; }

        public TransactionOptions Transactions { get; private set; }
 
        public bool EnsureQueuesExist { get; set; }
    }
}
