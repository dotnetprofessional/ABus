using System;
using ABus.Contracts;

namespace ABus
{
    public class Bus  :IBus
    {
        InboundMessageContext Context { get; set; }
        Pipeline Pipeline { get; set; }

        public Bus(InboundMessageContext context, Pipeline pipeline)
        {
            this.Context = context;
            this.Pipeline = pipeline;
        }

        public RawMessage CurrentMessage
        {
            get { return this.Context.RawMessage; }
        }

        public void Publish(object message)
        {
            this.Pipeline.SendOutboundMessage(this.Context, OutboundMessageContext.MessageIntent.Publish, message);
        }
         

        public void Send(object message)
        {
            this.Pipeline.SendOutboundMessage(this.Context, OutboundMessageContext.MessageIntent.Send, message);
        }

        public void Reply(object message)
        {
            this.Pipeline.SendOutboundMessage(this.Context, OutboundMessageContext.MessageIntent.Reply, message);
        }

        public void TerminateMessagePipeline()
        {
            this.Context.ShouldTerminatePipeline = true;
        }

    }
}