using System;
using System.Threading.Tasks;
using ABus.Contracts;

namespace ABus.Tasks.Outbound
{
    class CreateOutboundMessageTask : IPipelineOutboundMessageTask
    {
        public async Task InvokeAsync(OutboundMessageContext context, Func<Task> next)
        {
            if (string.IsNullOrEmpty(context.RawMessage.MessageId))
                context.RawMessage.MessageId = Guid.NewGuid().ToString();

            await next().ConfigureAwait(false); 
        }
    }
}
