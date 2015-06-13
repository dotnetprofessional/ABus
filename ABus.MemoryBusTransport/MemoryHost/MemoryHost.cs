using ABus.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABus.MemoryBusTransport.MemoryHost
{
    public class MemoryHost
    {
        public string HostUri { get; private set; }

        public MemoryHost(TransportDefinition transport)
        {
            HostUri = transport.Uri;
            Queue = new MemoryQueue();
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
