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

        public async Task PublishAsync(object message)
        {
            await this.Pipeline.SendOutboundMessage(this.Context, MessageIntent.Publish, message).ConfigureAwait(false);
        }
         

        public async Task SendAsync(object message)
        {
            await this.Pipeline.SendOutboundMessage(this.Context, MessageIntent.Send, message).ConfigureAwait(false);
        }

        public async Task ReplyAsync(object message)
        {
            await this.Pipeline.SendOutboundMessage(this.Context, MessageIntent.Reply, message).ConfigureAwait(false);
        }

        public void DeadLetterMessage()
        {
            this.CurrentMessage.State = MessageState.Deadlettered;
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