using System;

namespace ABus
{
    public class InitailizePipeline2 : IPipelineStartupTask
    {
        public void Invoke(PipelineContext context, Action next)
        {
            context.Trace.Information("Startup Pipeline2 task invoked!");
            next();
            context.Trace.Information("Startup Pipeline2 task ended!"); 
        }
    }
}