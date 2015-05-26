using System;

namespace ABus
{
    public interface IPipelineInboundMessageTask
    {
        void Invoke(InboundMessageContext context, Action next);
    }
}