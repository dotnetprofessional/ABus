using ABus.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using ABus.MemoryServiceBus.ServiceBus;

namespace ABus.MemoryServiceBus
{
    public class MemoryBusTransport : IMessageTransport
    {
        public event EventHandler<RawMessage> MessageReceived;
        ConcurrentDictionary<string, MemoryHost> HostInstances;

        public MemoryBusTransport()
        {
            HostInstances = new ConcurrentDictionary<string, MemoryHost>();
        }

        public void ConfigureHost(TransportDefinition transport)
        {
            HostInstances.AddOrUpdate(transport.Uri, key => new MemoryHost(transport), (key, oldValue) => oldValue);
        }

        public void Publish(QueueEndpoint endpoint, RawMessage message)
        {
            HostInstances[endpoint.Host].GetTopic(endpoint.Name).Send(new BrokeredMessage(endpoint.Host, endpoint.Name, message));
        }

        public void Send(QueueEndpoint endpoint, RawMessage message)
        {
            HostInstances[endpoint.Host].GetTopic(endpoint.Name).Send(new BrokeredMessage(endpoint.Host, endpoint.Name, message));
        }

        public void Send(QueueEndpoint endpoint, IEnumerable<RawMessage> messages)
        {
            var topic = HostInstances[endpoint.Host].GetTopic(endpoint.Name);
            foreach (var message in messages)
                topic.Send(new BrokeredMessage(endpoint.Host, endpoint.Name, message));
        }

        public async Task SendAsync(QueueEndpoint endpoint, RawMessage message)
        {
            await Task.Run(() => Send(endpoint, message));
        }

        public async Task SendAsync(QueueEndpoint endpoint, IEnumerable<RawMessage> messages)
        {
            await Task.Run(() => Send(endpoint, messages)).ConfigureAwait(false);
        }

        public async Task SubscribeAsync(QueueEndpoint endpoint, string subscriptionName)
        {
            await Task.Run(() => HostInstances[endpoint.Host].GetTopic(endpoint.Name).CreateSubscription(subscriptionName)).ConfigureAwait(false);
        }

        public void Subscribe(QueueEndpoint endpoint, string subscriptionName)
        {
            HostInstances[endpoint.Host].GetTopic(endpoint.Name).CreateSubscription(subscriptionName);
        }

        public async Task CreateQueueAsync(QueueEndpoint endpoint)
        {
            await Task.Run(() => HostInstances[endpoint.Host].CreateTopic(endpoint.Name)).ConfigureAwait(false);
        }

        public void CreateQueue(QueueEndpoint endpoint)
        {
            HostInstances[endpoint.Host].CreateTopic(endpoint.Name);
        }

        public async Task DeleteQueueAsync(QueueEndpoint endpoint)
        {
            await Task.Run(() => HostInstances[endpoint.Host].DeleteTopic(endpoint.Name)).ConfigureAwait(false);
        }

        public void DeleteQueue(QueueEndpoint endpoint)
        {
            HostInstances[endpoint.Host].DeleteTopic(endpoint.Name);
        }

        public async Task<bool> QueueExistsAsync(QueueEndpoint endpoint)
        {
            return await Task<bool>.Run(() => HostInstances[endpoint.Host].TopicExists(endpoint.Name)).ConfigureAwait(false);
        }

        public bool QueueExists(QueueEndpoint endpoint)
        {
            return HostInstances[endpoint.Host].TopicExists(endpoint.Name);
        }

        public Task DeferAsync(RawMessage message, TimeSpan timeToDelay)
        {
            throw new NotImplementedException();
        }
    }
}
