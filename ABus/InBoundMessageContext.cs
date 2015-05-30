using System;
using System.Collections.Generic;
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
            this.OutboundMessages = new List<RawMessage>();
        }
         
        public RawMessage RawMessage { get; private set; }

        public string SubscriptionName{ get; private set; } 

        public object TypeInstance { get; set; }

        public PipelineContext PipelineContext { get; private set; }

        public IBus Bus { get; set; }

        public List<RawMessage> OutboundMessages { get; set; }
    }
}