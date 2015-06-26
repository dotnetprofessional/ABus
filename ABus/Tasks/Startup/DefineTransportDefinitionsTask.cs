using System;
using System.Threading.Tasks;

namespace ABus.Tasks.Startup
{
    internal class DefineTransportDefinitionsTask : IPipelineStartupTask
    {
        public async Task InvokeAsync(PipelineContext context, Func<Task> next)
        {
            // TODO: Add code that can read a config to add Transport definitions that not specified in code.
            
            await next().ConfigureAwait(false);
        }
    }
}
