using System;
using System.Threading.Tasks;

namespace ABus.Sample
{
    public class CustomSecurityTask: IPipelineInboundMessageTask
    {
        public async Task InvokeAsync(InboundMessageContext context, Func<Task> next)
        {
            context.PipelineContext.Trace.Information("Authenticated request");
            await next().ConfigureAwait(false);
        }
    }

    public class CustomStartupTask : IPipelineStartupTask
    {
        public async Task InvokeAsync(PipelineContext context, Func<Task> next)
        {
            context.Trace.Information("Custom Startup Task");
            await next().ConfigureAwait(false);
        }
    }
}