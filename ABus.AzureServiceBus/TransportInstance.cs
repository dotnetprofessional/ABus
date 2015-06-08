using ABus.Contracts;
using Microsoft.ServiceBus;

namespace ABus.AzureServiceBus
{
    internal class TransportInstance : TransportDefinition
    {
        public NamespaceManager Namespace { get; set; }

        public string ConnectionString { get { return string.Format("Endpoint={0};{1}", this.Uri, this.Credentials); } }

    }
}