using System;

namespace ABus
{
    public interface IPipelineMessageTask
    {
        void Invoke(MessageContext context, Action next);
    }
}