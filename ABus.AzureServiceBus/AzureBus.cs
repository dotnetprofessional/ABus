using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Configuration;
using System.Threading.Tasks;
using ABus.Contracts;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace ABus.AzureServiceBus
{
    public class AzureBusTransport : IMessageTransport
    {
        public event EventHandler<RawMessage> MessageReceived;

        public AzureBusTransport(MetaDataCollection configuration)
        {
            this.Configuration = configuration;
            this.CreatedTopicClients = new Dictionary<string, TopicClient>();
            this.CreatedSubscriptionClients = new Dictionary<string, SubscriptionClient>();

            this.InitializeTransport();
        }

        NamespaceManager Namespace { get; set; }

        string ConnectionString { get; set; }

        public MetaDataCollection Configuration { get; set; }

        public QueueEndpoint Endpoint { get; set; }

        public void Publish(string queue, RawMessage message)
        {
            var client = this.GetTopicClient(queue);
        }

        public void Send(string queue, RawMessage message)
        {
            this.SendAsync(queue, message).Wait();
        }

        public void Send(string queue, IEnumerable<RawMessage> message)
        {
            this.SendAsync(queue, message).Wait();
        }

        public async Task SendAsync(string queue, IEnumerable<RawMessage> messages)
        {
            var client = this.GetTopicClient(queue);

            var waitingTasks = new List<Task>();
            foreach(var raw in messages)
                waitingTasks.Add(client.SendAsync(this.ConvertToBrokeredMessage(raw)));

            Task.WaitAll(waitingTasks.ToArray());
        }

        public async Task SendAsync(string queue, RawMessage message)
        {
            var client = this.GetTopicClient(queue);

            var brokeredMessage = this.ConvertToBrokeredMessage(message);
            await client.SendAsync(brokeredMessage);
        }

        public void Subscribe(string queue, string name)
        {
            var client = this.GetSubscriptionClient(queue, name);

            // Configure the callback options
            var options = new OnMessageOptions
            {
                AutoComplete = false,
                AutoRenewTimeout = TimeSpan.FromMinutes(1),
                MaxConcurrentCalls = 10,
            };
            client.PrefetchCount = 100;
            client.OnMessageAsync(async message =>
            {
                bool shouldAbandon = false;
                try
                {
                    // publish the message to the lister
                    if (this.MessageReceived != null)
                    {
                        this.MessageReceived(this, this.ConvertFromBrokeredMessage(message));
                    }

                    // Remove message from subscription
                    await message.CompleteAsync();
                }
                catch (Exception)
                {
                    // Indicates a problem, unlock message in subscription
                    shouldAbandon = true;
                }

                if(shouldAbandon)
                    await message.AbandonAsync();

            }, options);
        }

        bool IsInitialized { get; set; }

        void InitializeTransport()
        {
            var connectionStringKey = "AzureServiceBus.ConnectionString";
            // Ensure we have the connection string
            if (this.Configuration.Contains(connectionStringKey))
            {
                this.ConnectionString = this.Configuration[connectionStringKey].Value;
                this.Namespace = NamespaceManager.CreateFromConnectionString(this.ConnectionString);

                this.IsInitialized = true;
            }
        }

        Dictionary<string, TopicClient> CreatedTopicClients { get; set; }
        Dictionary<string, SubscriptionClient> CreatedSubscriptionClients { get; set; }

        /// <summary>
        /// A queue as used with Azure Service Bus is a topic which allows for a pub/sub model on a queue
        /// </summary>
        /// <param name="topic"></param>
        TopicClient GetTopicClient(string topic)
        {
            if (!this.CreatedTopicClients.ContainsKey(topic))
            {
                if (!this.Namespace.TopicExists(topic))
                    this.Namespace.CreateTopic(topic);

                var topicClient = TopicClient.CreateFromConnectionString(this.ConnectionString, topic);

                this.CreatedTopicClients.Add(topic, topicClient);
            }

            // return the topic client that has been stored
            return this.CreatedTopicClients[topic];
        }

        SubscriptionClient GetSubscriptionClient(string topic, string subscription)
        {
            if (!this.CreatedSubscriptionClients.ContainsKey(subscription))
            {
                if (!this.Namespace.TopicExists(topic))
                    throw new ArgumentException(string.Format("Unable to subscribe to topic {0} as it does not exist.", topic));

                var subscriptionClient = SubscriptionClient.CreateFromConnectionString(this.ConnectionString, topic, subscription);

                this.CreatedSubscriptionClients.Add(subscription, subscriptionClient);
            }

            // return the topic client that has been stored
            return this.CreatedSubscriptionClients[subscription];
        }

        BrokeredMessage ConvertToBrokeredMessage(RawMessage rawMessage)
        {
            var bm = new BrokeredMessage(rawMessage.Body);

            return bm;
        }

        RawMessage ConvertFromBrokeredMessage(BrokeredMessage brokeredMessage)
        {
            var msg = new RawMessage();
            msg.MessageId = brokeredMessage.MessageId;
            using (var stream = brokeredMessage.GetBody<Stream>())
            {
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int)stream.Length);

                // assign to message
                msg.Body = buffer;
            }

            return msg;
        }
    }
}
