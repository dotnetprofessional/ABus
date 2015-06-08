using System;

namespace ABus.Tasks.Outbound
{
    class SendMessageTask :IPipelineOutboundMessageTask
    {
        public void Invoke(OutboundMessageContext context, Action next)
        {
            // If there is no inbound message (RawMessage) then dispatch message immediately
            if (context.InboundMessageContext.RawMessage.Body == null)
                context.PipelineContext.DispatchMessage(context.RawMessage);
            else
                // Add to the batch of messages to be handled in a transaction
                context.InboundMessageContext.OutboundMessages.Add(context.RawMessage);

            next();
        }
    }
}
