using System;

namespace ABus.Sample
{
    public class CustomSecurityTask: IPipelineInboundMessageTask
    {
        public void Invoke(InboundMessageContext context, Action next)
        {
            context.PipelineContext.Trace.Information("Authenticated request");
            next();
        }
    }

    public class CustomStartupTask : IPipelineStartupTask
    {
        public void Invoke(PipelineContext context, Action next)
        {
            context.Trace.Information("Custom Startup Task");
            next();
        }
    }
}