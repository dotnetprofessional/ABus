using System;
using ABus.Contracts;

namespace ABus.Tasks.Outbound
{
    class SendMessageTask :IPipelineOutboundMessageTask
    {
        public void Invoke(OutboundMessageContext context, Action next)
        {
            context.InboundMessageContext.TransactionManager.AddItem(context.RawMessage);
             
            next();
        }
    }
}
