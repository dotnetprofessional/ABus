using ABus.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABus.MemoryServiceBus.ServiceBus
{
    public class SubscriptionOptions
    {
        public QueueEndpoint Endpoint { get; set; }
        public int MaxConcurrentCalls { get; set; }
        public Func<BrokeredMessage, Task> OnMessage;
    }

    public class Subscription
    {
        public Subscription(string hostUri, string topicName, string subscriptionName)
        {
            HostUri = hostUri;
            TopicName = topicName;
            Name = subscriptionName;
        }

        public void Subscribe(SubscriptionOptions options)
        {
            throw new NotImplementedException();
        }

        public string HostUri { get; private set; }
        public string TopicName { get; private set; }
        public string Name { get; private set; }
    }
}
