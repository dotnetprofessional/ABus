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
        public InboundMessageContext(string source, RawMessage rawMessage, PipelineContext pipelineContext)
        {
            if (!string.IsNullOrEmpty(source))
            {
                // Source should have the format [queue]:[subscription]
                var sourceParts = source.Split(':');
                this.Queue = sourceParts[0];
                this.SubscriptionName = sourceParts[1];
            }
            this.RawMessage = rawMessage;
            this.PipelineContext = pipelineContext;
            this.OutboundMessages = new List<RawMessage>();
            this.ShouldTerminatePipeline = false;
        }
         
        public RawMessage RawMessage { get; private set; }

        public string Queue { get; set; }

        public string SubscriptionName{ get; private set; } 

        public object TypeInstance { get; set; }

        public PipelineContext PipelineContext { get; private set; }

        public IBus Bus { get; set; }

        public List<RawMessage> OutboundMessages { get; set; }

        public bool ShouldTerminatePipeline { get; set; }
    }
}