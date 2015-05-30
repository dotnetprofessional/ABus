using System;

namespace ABus
{
    public class InitailizePipeline3 : IPipelineStartupTask
    {
        public void Invoke(PipelineContext context, Action next)
        {
            context.Trace.Information("Startup Pipeline3 task invoked!");
            next();
            context.Trace.Information("Startup Pipeline task3 ended!");
        }
    }
}