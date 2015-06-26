using System;
using System.Threading.Tasks;

namespace ABus
{
    public interface IPipelineOutboundMessageTask
    {
        Task InvokeAsync(OutboundMessageContext context, Func<Task> next);
    }
}