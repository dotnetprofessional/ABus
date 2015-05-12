using System;
using System.Diagnostics;

namespace ABus
{
    public class InitailizePipeline : IPipelineInitializationTask
    {
        public void Invoke(PipelineContext context, Action next)
        {
            Trace.WriteLine("Initialization Pipeline task started!");
            Trace.WriteLine("Initialization Pipeline task invoked!");
            next();
            Trace.WriteLine("Initialization Pipeline task ended!");
        }
    }

    public class InitailizePipeline2 : IPipelineInitializationTask
    {
        public void Invoke(PipelineContext context, Action next)
        {
            Trace.WriteLine("Initialization Pipeline2 task started!");
            Trace.WriteLine("Initialization Pipeline2 task invoked!");
            next();
            Trace.WriteLine("Initialization Pipeline2 task ended!");
        }
    }
}
