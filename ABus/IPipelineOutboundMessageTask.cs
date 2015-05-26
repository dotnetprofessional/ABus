using System;

namespace ABus
{
    public interface IPipelineOutboundMessageTask
    {
        void Invoke(OutboundMessageContext context, Action next);
    }
}