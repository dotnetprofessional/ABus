using System;
using ABus.Contracts;

namespace ABus
{
    /// <summary>
    /// Contains the state of the message
    /// </summary>
    public class InboundMessageContext
    {
        public InboundMessageContext(string subscriptionName, RawMessage rawMessage, PipelineContext pipelineContext)
        {
            this.SubscriptionName = subscriptionName;
            this.RawMessage = rawMessage;
            this.PipelineContext = pipelineContext;
        }
         
        public RawMessage RawMessage { get; private set; }

        public string SubscriptionName{ get; private set; } 

        public object TypeInstance { get; set; }

        public PipelineContext PipelineContext { get; private set; }

        public IBus Bus { get; set; }

        public IManageOutboundMessages TransactionManager { get; set; }
    }

    /// <summary>
    /// Contains the state of the message
    /// </summary>
    public class OutboundMessageContext
    {
        public enum MessageIntent
        {
            Send,
            Reply,
            Publish
        }

        public OutboundMessageContext(QueueEndpoint queue, object messageInstance, PipelineContext pipelineContext, InboundMessageContext inboundMessageContext)
        {
            this.Queue = queue;
            this.MessageInstance = messageInstance;
            this.PipelineContext = pipelineContext;
            this.InboundMessageContext = inboundMessageContext;
            this.RawMessage = new RawMessage();
        }

        public RawMessage RawMessage { get; private set; }

        public QueueEndpoint Queue { get; private set; }

        public object MessageInstance { get; set; }

        public PipelineContext PipelineContext { get; private set; }

        public InboundMessageContext InboundMessageContext { get; private set; }
    }
}