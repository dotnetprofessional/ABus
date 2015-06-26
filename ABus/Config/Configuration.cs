using System.Collections.Generic;
using System.Collections.ObjectModel;
using ABus.Config.MessageEndpoint;
using ABus.Config.Pipeline;
using ABus.Config.Transactions;
using ABus.Contracts;
using Microsoft.Practices.ServiceLocation;

namespace ABus.Config
{
    public class Configuration
    {
        public Configuration()
        {
            this.Pipeline = new PipelineConfiguration();
            this.AvailableTransports = new AvailableTransportCollection();
            this.MessageEndpointDefinitions = new List<MessageEndpointDefinition>();
            this.Transactions = new TransactionOptions();
        }

        public IServiceLocator Container { get; set; }

        public PipelineConfiguration Pipeline { get; private set; }

        public AvailableTransportCollection AvailableTransports { get; private set; }

        public List<MessageEndpointDefinition> MessageEndpointDefinitions { get; private set; }

        public TransactionOptions Transactions { get; private set; }
 
        public bool EnsureQueuesExist { get; set; }

        public MessageEndpointDefinition ReplyToQueue { get; set; }
    }

    public class AvailableTransportCollection : KeyedCollection<string, TransportDefinition>
    {
        protected override string GetKeyForItem(TransportDefinition item)
        {
            return item.Name;
        }
    }
}
