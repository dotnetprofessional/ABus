using System;
using System.Collections.Generic;
using System.IO;
using ABus.Contracts;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace ABus.AzureServiceBus
{
    public class AzureBusTransport : IMessageTransport
    {
        public AzureBusTransport(MetaDataCollection configuration)
        {
            this.Configuration = configuration;
            this.CreatedTopicClients = new Dictionary<string, TopicClient>();

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
            var client = this.GetTopicClient(queue);

            var brokeredMessage = new BrokeredMessage(message.Body);
            client.SendAsync(brokeredMessage);

        }

        public void Subscribe(string queue, string name)
        {
            var client = this.GetTopicClient(queue);
            
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

        BrokeredMessage ConvertToBrokeredMessage(RawMessage rawMessage)
        {
            var bm = new BrokeredMessage(rawMessage.Body);

            return bm;
        }
    }
}
