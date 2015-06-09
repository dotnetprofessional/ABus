using ABus.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace ABus.InMemoryServiceBus
{
    public class InMemoryBusTransport : IMessageTransport
    {
        public event EventHandler<RawMessage> MessageReceived;
        ConcurrentDictionary<string, TransportDefinition> HostInstances;

        public InMemoryBusTransport()
        {
            HostInstances = new ConcurrentDictionary<string, TransportDefinition>();
        }

        public void ConfigureHost(TransportDefinition transport)
        {
            var hostInstance = new TransportDefinition { Uri = transport.Uri, Credentials = transport.Credentials };
            HostInstances.AddOrUpdate(transport.Uri, hostInstance, (key, oldValue) => oldValue);
        }

        public void Publish(QueueEndpoint endpoint, RawMessage message)
        {
            throw new NotImplementedException();
        }

        public void Send(QueueEndpoint endpoint, RawMessage message)
        {
            this.SendAsync(endpoint, message).Wait();
        }

        public void Send(QueueEndpoint endpoint, IEnumerable<RawMessage> message)
        {
            this.SendAsync(endpoint, message).Wait();
        }

        public async Task SendAsync(QueueEndpoint endpoint, RawMessage message)
        {
            throw new NotImplementedException();
        }

        public async Task SendAsync(QueueEndpoint endpoint, IEnumerable<RawMessage> message)
        {
            throw new NotImplementedException();
        }

        public async Task SubscribeAsync(QueueEndpoint endpoint, string subscriptionName)
        {
            throw new NotImplementedException();
        }

        public async Task CreateQueue(QueueEndpoint endpoint)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteQueue(QueueEndpoint endpoint)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> QueueExists(QueueEndpoint endpoint)
        {
            throw new NotImplementedException();
        }

        IBusQueue Queue;
        //BlockingCollection<int> Queue;
    }
}
