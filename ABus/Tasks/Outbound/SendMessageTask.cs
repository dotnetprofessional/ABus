using System;
using ABus.Contracts;

namespace ABus.Tasks.Outbound
{
    class SendMessageTask :IPipelineOutboundMessageTask
    {
        public void Invoke(OutboundMessageContext context, Action next)
        {
            context.InboundMessageContext.OutboundMessages.Add(context.RawMessage);
             
            next();
        }
    }
}
