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
        public event EventHandler<TransportException> ExceptionOccured;
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
            await Task.Run(() => Subscribe(endpoint, subscriptionName)).ConfigureAwait(false);
        }

        public void Subscribe(QueueEndpoint endpoint, string subscriptionName)
        {
            SubscriptionOptions options = new SubscriptionOptions
            {
                OnMessageReceived = (message) =>
                {
                    TransportException transportException = null;
                    bool shouldAbandon = false;
                    try
                    {
                        MessageReceived("", message.Message);
                    }
                    catch (Exception ex)
                    {
                        transportException = new TransportException("OnMessage pump", ex);
                        shouldAbandon = true;
                    }
                    if (shouldAbandon)
                    {
                        // TODO: How to abandon message

                        if (this.ExceptionOccured != null && transportException != null)
                            this.ExceptionOccured(this, transportException);
                    }
                }
            };
            var subscription = HostInstances[endpoint.Host].GetTopic(endpoint.Name).CreateSubscription(subscriptionName, options);
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
            await Task.Run(() => DeleteQueue(endpoint)).ConfigureAwait(false);
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
