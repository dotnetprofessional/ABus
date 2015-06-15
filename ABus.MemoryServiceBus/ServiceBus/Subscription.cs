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
        //public int MaxConcurrentCalls { get; set; }
        //public Func<BrokeredMessage, Task> OnMessageReceivedAsync;
        public Action<BrokeredMessage> OnMessageReceived;
    }

    public class Subscription
    {
        public Subscription(string hostUri, string topicName, string subscriptionName, SubscriptionOptions options)
        {
            HostUri = hostUri;
            TopicName = topicName;
            Name = subscriptionName;
            Options = options;
        }

        public string HostUri { get; private set; }
        public string TopicName { get; private set; }
        public string Name { get; private set; }
        public Action<BrokeredMessage> OnMessageReceived { get; set; }
        SubscriptionOptions Options;
    }
}
