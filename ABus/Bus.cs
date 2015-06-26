using System;
using System.Threading.Tasks;
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
            Task.Run(async ()=> await this.Pipeline.SendOutboundMessage(this.Context, OutboundMessageContext.MessageIntent.Publish, message).ConfigureAwait(false)).Wait();
        }
         

        public void Send(object message)
        {
            Task.Run(async ()=> await this.Pipeline.SendOutboundMessage(this.Context, OutboundMessageContext.MessageIntent.Send, message).ConfigureAwait(false)).Wait();
        }

        public void Reply(object message)
        {
            Task.Run(async ()=> await this.Pipeline.SendOutboundMessage(this.Context, OutboundMessageContext.MessageIntent.Reply, message).ConfigureAwait(false)).Wait();
        }

        public void DeadLetterMessage()
        {
            this.CurrentMessage.MetaData.Add(new MetaData{Name = StandardMetaData.ShouldDeadLetterMessage, Value = ""});
        }

        public void TerminateMessagePipeline()
        {
            this.Context.ShouldTerminatePipeline = true;
        }

        public ABusTraceSource Trace
        {
            get { return this.Pipeline.Trace; }
        }

        public void Dispose()
        {
            
        }
    }
}