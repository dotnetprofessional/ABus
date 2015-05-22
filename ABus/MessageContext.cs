using ABus.Contracts;

namespace ABus
{
    /// <summary>
    /// Contains the state of the message
    /// </summary>
    public class MessageContext
    {
        public MessageContext(string subscriptionName, RawMessage rawMessage, PipelineContext pipelineContext)
        {
            SubscriptionName = subscriptionName;
            RawMessage = rawMessage;
            PipelineContext = pipelineContext;
        }
         
        public RawMessage RawMessage { get; private set; }

        public string SubscriptionName{ get; private set; }

        public object TypeInstance { get; set; }

        public IBus Bus { get; set; }

        public PipelineContext PipelineContext { get; private set; }
    }
}