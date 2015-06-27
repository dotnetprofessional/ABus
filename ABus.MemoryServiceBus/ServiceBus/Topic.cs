using ABus.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABus.MemoryServiceBus.ServiceBus
{
    public class Topic
    {
        internal Topic(string hostUri, string topicName, MemoryQueue queue, Topic errorTopic = null)
        {
            HostUri = hostUri;
            Name = topicName;
            Subscriptions = new ConcurrentDictionary<string, Subscription>();
            Queue = queue;
        }

        public void Send(BrokeredMessage message)
        {
            Queue.Send(message);
        }

        public Subscription GetSubscription(string subscriptionName)
        {
            Subscription subscription;
            if (!Subscriptions.TryGetValue(subscriptionName, out subscription))
                throw new Exception(string.Format("Subscription {0} does not exist", subscriptionName));
            return subscription;
        }

        public Subscription CreateSubscription(string subscriptionName, SubscriptionOptions options)
        {
            Subscription subscription = null;
            Subscriptions.AddOrUpdate(subscriptionName,
                (key) => subscription = new Subscription(HostUri, Name, subscriptionName, options),
                (key, oldValue) => subscription = oldValue);
            return subscription;
        }


        public ConcurrentDictionary<string, Subscription> Subscriptions { get; private set; }
        MemoryQueue Queue;
        public string HostUri { get; private set; }
        public string Name { get; private set; }
    }
}
