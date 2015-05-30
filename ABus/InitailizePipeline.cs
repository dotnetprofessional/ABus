using System;
using System.Diagnostics;

namespace ABus
{
    public class InitailizePipeline : IPipelineStartupTask
    {
        public void Invoke(PipelineContext context, Action next)
        {
            context.Trace.Information("Startup Pipeline task invoked!");
            next();
            context.Trace.Information("Startup Pipeline task ended!");
        }
    }
}
