using ABus.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABus.MemoryServiceBus.ServiceBus
{
    public class MemoryHost
    {
        public TransportDefinition Definition { get; private set; }
        public string HostUri { get; private set; }

        public MemoryHost(TransportDefinition transport)
        {
            Definition = transport;
            HostUri = transport.Uri;
            Queue = new MemoryQueue(MessageReceived);
            Topics = new ConcurrentDictionary<string, Topic>();
        }

        void MessageReceived(BrokeredMessage message)
        {
            if (message.Host != HostUri)
                throw new ArgumentException("Invalid message host");

            GetTopic(message.Topic).Subscriptions.AsParallel().ForAll(pair =>
            {
                Subscription subscription = pair.Value;
                try
                {
                    if(subscription.OnMessageReceived != null)
                        subscription.OnMessageReceived(message.Clone());
                }
                catch (Exception ex)
                {
                    // TODO: handle failed messages
                }
            });
        }

        public Topic GetTopic(string topicName)
        {
            Topic topic;
            if (!Topics.TryGetValue(topicName, out topic))
                throw new Exception(string.Format("Topic {0} does not exist", topicName));
            return topic;
        }

        public Topic CreateTopic(string topicName)
        {
            Topic topic = null;
            Topics.AddOrUpdate(topicName, key => topic = new Topic(HostUri, topicName, Queue), (key, oldValue) => topic = oldValue);
            return topic;
        }

        public void DeleteTopic(string topicName)
        {
            Topic topic;
            Topics.TryRemove(topicName, out topic);
        }

        public bool TopicExists(string topicName)
        {
            return Topics.ContainsKey(topicName);
        }

        Subscription GetSubscription(string topicName, string subscriptionName)
        {
            return GetTopic(topicName).GetSubscription(subscriptionName);
        }

        //public void Subscribe(

        //ConcurrentQueue<RawMessage> Queue;
        MemoryQueue Queue;
        ConcurrentDictionary<string, Topic> Topics;

    }

}
