using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ABus.Contracts;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using MessageState = ABus.Contracts.MessageState;

namespace ABus.AzureServiceBus
{
    public class AzureBusTransport : IMessageTransport
    {
        public event EventHandler<RawMessage> MessageReceived;

        public event EventHandler<TransportException> ExceptionOccured;

        readonly object topicClientLock = new object();

        public AzureBusTransport()
        {
            this.HostInstances = new Dictionary<string, TransportInstance>();
            this.CreatedTopicClients = new Dictionary<string, TopicClient>();
            this.CreatedSubscriptionClients = new Dictionary<string, SubscriptionClient>();
        }

        Dictionary<string, TransportInstance> HostInstances { get; set; }
         
        public void ConfigureHost(TransportDefinition transport)
        {
            if (!this.HostInstances.ContainsKey(transport.Uri))
            {
                var hostInstance = new TransportInstance { Uri = transport.Uri, Credentials = transport.Credentials };
                var ns = NamespaceManager.CreateFromConnectionString(hostInstance.ConnectionString);
                hostInstance.Namespace = ns;
                hostInstance.Definition = transport;

                this.HostInstances.Add(transport.Uri, hostInstance);
            }
        }

        public async Task PublishAsync(QueueEndpoint endpoint, RawMessage message)
        {
            await this.SendAsync(endpoint, message).ConfigureAwait(false);
        }

        public async Task SendAsync(QueueEndpoint endpoint, IEnumerable<RawMessage> messages)
        {
            var client = this.GetTopicClient(endpoint);

            var waitingTasks = new List<Task>();
            foreach(var raw in messages)
                waitingTasks.Add(client.SendAsync(this.ConvertToBrokeredMessage(raw)));

            await Task.WhenAll(waitingTasks.ToArray()).ConfigureAwait(false);
        }

        public async Task SendAsync(QueueEndpoint endpoint, RawMessage message)
        {
            var client = this.GetTopicClient(endpoint);
            if (endpoint.SubQueueName != "")
                message.MetaData.Add(new MetaData {Name = AzureTransportMetaData.ForSubscriber, Value = endpoint.SubQueueName});

            var brokeredMessage = this.ConvertToBrokeredMessage(message);
            await client.SendAsync(brokeredMessage).ConfigureAwait(false);
        }

        public async Task SubscribeAsync(QueueEndpoint endpoint, string subscriptionName, string filter)
        {
            var client = await this.GetSubscriptionClient(endpoint, subscriptionName, filter).ConfigureAwait(false);

            // Configure the callback options
            var options = new OnMessageOptions
            {
                AutoComplete = false,
                AutoRenewTimeout = TimeSpan.FromMinutes(10),
                MaxConcurrentCalls = 10,
            };
            client.PrefetchCount = 100;
            client.OnMessageAsync(async message =>
            {
                TransportException transportException = null;
                bool shouldAbandon = false;
                try
                {
                    RawMessage rawMessage = null;
                    // publish the message to the listener
                    if (this.MessageReceived != null)
                    {
                        var source = string.Format("{0}:{1}", endpoint.Name, subscriptionName);
                        rawMessage = this.ConvertFromBrokeredMessage(message);
                        this.MessageReceived(source, rawMessage);
                    }

                    if (rawMessage.State == MessageState.Deadlettered)
                    {
                        // Move message to dead letter queue
                        this.UpdateMessageMetaData(message, rawMessage);
                        await message.DeadLetterAsync(message.Properties).ConfigureAwait(false);
                    }
                    else
                        // Remove message from subscription
                        await message.CompleteAsync().ConfigureAwait(false);

                }
                catch (Exception ex)
                {
                    // Capture the exception that has occured so that it can be evented out
                    transportException = new TransportException("OnMessage pump", ex);
                    // Indicates a problem, unlock message in subscription
                    shouldAbandon = true;
                }

                if (shouldAbandon)
                {
                    // Abandon the message first
                    await message.AbandonAsync().ConfigureAwait(false);
                    // Now event the exception
                    if (this.ExceptionOccured != null && transportException != null)
                        this.ExceptionOccured(this, transportException);
                }  

            }, options);
        }

        public Task SubscribeAsync(QueueEndpoint endpoint, string subscriptionName)
        {
            return this.SubscribeAsync(endpoint, subscriptionName, null);
        }

        public async Task CreateQueueAsync(QueueEndpoint endpoint)
        {
            var host = this.HostInstances[endpoint.Host];
            var topicDescription = new TopicDescription(endpoint.Name);

            // Create the topic
            await host.Namespace.CreateTopicAsync(topicDescription).ConfigureAwait(false);

            // When creating special queues such as Audit and Errors then additional steps are needed

            // Error Queue
            if (endpoint.Name == host.ErrorQueue)
            {
                //  Neeed to create a default subscription so the messages dont disappear!
                var subscriptionConfig = new SubscriptionDescription(endpoint.Name, "log")
                {
                    LockDuration = TimeSpan.FromSeconds(300), // 5 mins
                    EnableDeadLetteringOnMessageExpiration = true,
                    EnableDeadLetteringOnFilterEvaluationExceptions = true,
                };
                await host.Namespace.CreateSubscriptionAsync(subscriptionConfig).ConfigureAwait(false);

            }

            // Audit Queue
            // If an audit enabled create a subscription to forward all mesages to the audit queue
            if (host.EnableAuditing)
            {
                // Ensure we dont try to audit the audit queue!!
                if (endpoint.Name != host.AuditQueue)
                {
                    var subscriptionConfig = new SubscriptionDescription(endpoint.Name, "Audit")
                    {
                        ForwardTo = host.AuditQueue
                    };
                    await host.Namespace.CreateSubscriptionAsync(subscriptionConfig).ConfigureAwait(false);
                }
                else
                {
                    // When creating the audit queue, a default subscription is needed to hold the messages
                    var subscriptionConfig = new SubscriptionDescription(endpoint.Name, "log")
                    {
                        LockDuration = TimeSpan.FromSeconds(300), // 5 mins
                        EnableDeadLetteringOnMessageExpiration = true,
                        EnableDeadLetteringOnFilterEvaluationExceptions = true,
                    };
                    await host.Namespace.CreateSubscriptionAsync(subscriptionConfig).ConfigureAwait(false);
                }
            }
        }

        public async Task DeleteQueueAsync(QueueEndpoint endpoint)
        {
            await this.HostInstances[endpoint.Host].Namespace.DeleteTopicAsync(endpoint.Name);
        }

        public Task<bool> QueueExistsAsync(QueueEndpoint endpoint)
        {
            return this.HostInstances[endpoint.Host].Namespace.TopicExistsAsync(endpoint.Name);
        }

        public Task DeferAsync(QueueEndpoint endpoint, RawMessage message, TimeSpan timeToDelay)
        {
            throw new NotImplementedException();
        }

        void UpdateMessageMetaData(BrokeredMessage message, RawMessage rawMessage)
        {
            foreach (var m in rawMessage.MetaData)
                if (!message.Properties.ContainsKey(m.Name))
                    message.Properties.Add(m.Name, m.Value);

        }

        Dictionary<string, TopicClient> CreatedTopicClients { get; set; }
        Dictionary<string, SubscriptionClient> CreatedSubscriptionClients { get; set; }

        /// <summary>
        /// A queue as used with Azure Service Bus is a topic which allows for a pub/sub model on a queue
        /// </summary>
        /// <param name="topic"></param>
        TopicClient GetTopicClient(QueueEndpoint endpoint)
        {
            var host = this.HostInstances[endpoint.Host];
            var ns = host.Namespace;
            var topic = endpoint.Name;

            // As topic clients are not created during startup its possible for 
            // multiple threads to request the same topic client while its being obtained.
            // There will be a tempory slow down when a new topic client is requested for the first time.
            // However this should not be noticable for most systems.
            lock (topicClientLock)
            {
                if (!this.CreatedTopicClients.ContainsKey(topic))
                {
                    if (!ns.TopicExists(topic))
                    {
                        try
                        {
                            ns.CreateTopic(topic);
                        }
                        catch (MessagingEntityAlreadyExistsException)
                        {
                            // Case when topic is created by external process
                        }
                    }

                    var topicClient = TopicClient.CreateFromConnectionString(host.ConnectionString, topic);

                    this.CreatedTopicClients.Add(topic, topicClient);
                }
            }
            // return the topic client that has been stored
            return this.CreatedTopicClients[topic];
        }

        async Task<SubscriptionClient> GetSubscriptionClient(QueueEndpoint endpoint, string subscription, string filter)
        {
            // Azure Service Bus doesnt support using dot notation when using filters for properties
            // As such we need to strip dots and replace them with underscores for any properties that are used in filters
            subscription = subscription.Replace('.', '_');

            var host = this.HostInstances[endpoint.Host];
            var ns = host.Namespace;

            var topic = endpoint.Name;

            SubscriptionClient subscriptionClient;
            if (!this.CreatedSubscriptionClients.TryGetValue(subscription, out subscriptionClient))
            {
                if (!ns.TopicExists(topic))
                    throw new ArgumentException(string.Format("Unable to subscribe to topic {0} as it does not exist.", topic));

                // Now check if the subscription already exists if not create it
                if (!ns.SubscriptionExists(topic, subscription))
                {
                    var subscriptionConfig = new SubscriptionDescription(topic, subscription)
                    {
                        DefaultMessageTimeToLive = TimeSpan.FromSeconds(86400), // 24hrs
                        LockDuration = TimeSpan.FromSeconds(300), // 5 mins
                        EnableDeadLetteringOnMessageExpiration = true,
                        EnableDeadLetteringOnFilterEvaluationExceptions = true,
                        //ForwardDeadLetteredMessagesTo = host.ErrorQueue,
                    };

                    // Define a custom filter that supports sending messages directly to a subscription
                    var subscriptionFilter = string.Format("{0} is null OR {0}={1}", AzureTransportMetaData.ForSubscriber, subscription);

                    if (!string.IsNullOrEmpty(filter))
                        subscriptionFilter += " AND " + filter;
                    
                    var sqlFilter = new SqlFilter(subscriptionFilter);

                    await ns.CreateSubscriptionAsync(subscriptionConfig, sqlFilter).ConfigureAwait(false);
                }

                subscriptionClient = SubscriptionClient.CreateFromConnectionString(host.ConnectionString, topic, subscription, ReceiveMode.PeekLock);

                this.CreatedSubscriptionClients.Add(subscription, subscriptionClient);
            }

            // return the topic client that has been stored
            return subscriptionClient;
        }

        BrokeredMessage ConvertToBrokeredMessage(RawMessage rawMessage)
        {
            var bm = new BrokeredMessage(new MemoryStream(rawMessage.Body), true);
            foreach (var m in rawMessage.MetaData)
            {
                if (m.Name != StandardMetaData.ContentType)
                    bm.Properties.Add(m.Name, m.Value);
            }

            // Transfer system properties
            bm.MessageId = rawMessage.MessageId;
            bm.CorrelationId = rawMessage.CorrelationId;
            bm.ContentType = rawMessage.MetaData[StandardMetaData.ContentType].Value;

            return bm;
        }

        RawMessage ConvertFromBrokeredMessage(BrokeredMessage brokeredMessage)
        {
            var msg = new RawMessage();
            msg.MessageId = brokeredMessage.MessageId;
            msg.CorrelationId = brokeredMessage.CorrelationId;
            if (brokeredMessage.ContentType != null)
                msg.MetaData.Add(new MetaData { Name = StandardMetaData.ContentType, Value = brokeredMessage.ContentType });

            // Transfer meta data
            foreach (var p in brokeredMessage.Properties)
                msg.MetaData.Add(new MetaData { Name = p.Key, Value = p.Value as string });

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
