using System;
using System.Threading.Tasks;

namespace ABus.Tasks.Outbound
{
    class SendMessageTask :IPipelineOutboundMessageTask
    {
        public async Task InvokeAsync(OutboundMessageContext context, Func<Task> next)
        {
            // If there is no inbound message (RawMessage) then dispatch message immediately
            if (context.InboundMessageContext.RawMessage.Body == null)
                await context.PipelineContext.DispatchMessage(context.RawMessage).ConfigureAwait(false);
            else
                // Add to the batch of messages to be handled in a transaction
                context.InboundMessageContext.OutboundMessages.Add(context.RawMessage);

            await next().ConfigureAwait(false);
        }
    }
}
