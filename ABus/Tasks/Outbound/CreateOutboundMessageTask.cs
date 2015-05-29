using System;
using ABus.Contracts;

namespace ABus.Tasks.Outbound
{
    class CreateOutboundMessageTask : IPipelineOutboundMessageTask
    {
        public void Invoke(OutboundMessageContext context, Action next)
        {
            context.RawMessage.MessageId = Guid.NewGuid().ToString();

            next(); 
        }
    }
}
