using System;

namespace ABus
{
    public class InitailizePipeline4 : IPipelineStartupTask
    {
        public void Invoke(PipelineContext context, Action next)
        {
            context.Trace.Information("Startup Pipeline4 task invoked!");
            next();
            context.Trace.Information("Startup Pipeline4 task ended!");
        }
    }
}