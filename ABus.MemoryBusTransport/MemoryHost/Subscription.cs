using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABus.MemoryBusTransport.MemoryHost
{
    /*
    class SubscriptionOptions
    {
        public QueueEndpoint Endpoint { get; set; }
        public int MaxConcurrentCalls { get; set; }
        public Func<BrokeredMessage, Task> OnMessage;
    }
    */

    public class Subscription
    {
        public Subscription(string hostUri, string topicName, string subscriptionName)
        {
            HostUri = hostUri;
            TopicName = topicName;
            Name = subscriptionName;
        }

        string HostUri;
        string TopicName;
        string Name;
    }
}
