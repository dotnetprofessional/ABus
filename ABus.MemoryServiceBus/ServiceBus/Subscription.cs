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
        public Topic ErrorTopic { get; set; }
        public Action<BrokeredMessage> OnMessageReceived { get; set; }
    }

    public class Subscription
    {
        public Subscription(string hostUri, string topicName, string subscriptionName, SubscriptionOptions options)
        {
            HostUri = hostUri;
            TopicName = topicName;
            Name = subscriptionName;
            ErrorTopic = options.ErrorTopic;
            OnMessageReceived = options.OnMessageReceived;
        }

        public string HostUri { get; private set; }
        public string TopicName { get; private set; }
        public string Name { get; private set; }
        public Topic ErrorTopic { get; set; }
        public Action<BrokeredMessage> OnMessageReceived { get; set; }
    }
}
