using System;
using System.Diagnostics;

namespace ABus
{
    public class InitailizePipeline : IPipelineInitializationTask
    {
        public void Invoke(PipelineContext context, Action next)
        {
            Trace.WriteLine("Initialization Pipeline task invoked!");
        }
    }
}
