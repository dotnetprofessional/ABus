using System;
using System.Threading.Tasks;

namespace ABus
{
    public interface IPipelineInboundMessageTask
    {
        Task InvokeAsync(InboundMessageContext context, Func<Task> next);
    }
}