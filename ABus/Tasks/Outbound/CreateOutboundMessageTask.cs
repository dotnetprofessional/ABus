using System;
using ABus.Contracts;

namespace ABus.Tasks.Outbound
{
    class CreateOutboundMessageTask : IPipelineOutboundMessageTask
    {
        public void Invoke(OutboundMessageContext context, Action next)
        {
            if (string.IsNullOrEmpty(context.RawMessage.MessageId))
                context.RawMessage.MessageId = Guid.NewGuid().ToString();

            next(); 
        }
    }
}
