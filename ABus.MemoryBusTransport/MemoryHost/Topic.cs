using ABus.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABus.MemoryBusTransport.MemoryHost
{
    public class Topic
    {
        internal Topic(string hostUri, string topicName, MemoryQueue queue)
        {
            HostUri = hostUri;
            Name = topicName;
        }

        internal void Send(BrokeredMessage message)
        {
            Queue.Send(message);
        }

        internal Subscription GetSubscription(string subscriptionName)
        {
            Subscription subscription;
            if (!Subscriptions.TryGetValue(subscriptionName, out subscription))
                throw new Exception(string.Format("Subscription {0} does not exist", subscriptionName));
            return subscription;
        }

        internal Subscription CreateSubscription(string subscriptionName)
        {
            Subscription subscription = null;
            Subscriptions.AddOrUpdate(subscriptionName, key => subscription = new Subscription(HostUri, Name, subscriptionName), (key, oldValue) => subscription = oldValue);
            return subscription;
        }


        ConcurrentDictionary<string, Subscription> Subscriptions;
        MemoryQueue Queue;
        string HostUri;
        string Name;
    }
}
