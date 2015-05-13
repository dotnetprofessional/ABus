using System;
using System.Diagnostics;

namespace ABus
{
    public class InitailizePipeline : IPipelineStartupTask
    {
        public void Invoke(PipelineContext context, Action next)
        {
            Trace.WriteLine("Startup Pipeline task invoked!");
            next();
            Trace.WriteLine("Startup Pipeline task ended!");
        }
    }

    public class InitailizePipeline2 : IPipelineStartupTask
    {
        public void Invoke(PipelineContext context, Action next)
        {
            Trace.WriteLine("Startup Pipeline2 task invoked!");
            next();
            Trace.WriteLine("Startup Pipeline2 task ended!");
        }
    }

    public class InboundMessageTask : IPipelineMessageTask
    {
        public void Invoke(MessageContext context, Action next)
        {
            Trace.WriteLine("Message Pipeline1 task invoked!");
            next();
            Trace.WriteLine("Message Pipeline1 task ended!");
        }
    }

    public class InitailizePipeline3 : IPipelineStartupTask
    {
        public void Invoke(PipelineContext context, Action next)
        {
            Trace.WriteLine("Startup Pipeline3 task invoked!");
            next();
            Trace.WriteLine("Startup Pipeline task3 ended!");
        }
    }

    public class InitailizePipeline4 : IPipelineStartupTask
    {
        public void Invoke(PipelineContext context, Action next)
        {
            Trace.WriteLine("Startup Pipeline4 task invoked!");
            next();
            Trace.WriteLine("Startup Pipeline4 task ended!");
        }
    }
}
