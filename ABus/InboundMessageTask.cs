using System;

namespace ABus
{
    public class InboundMessageTask : IPipelineInboundMessageTask
    {
        public void Invoke(InboundMessageContext context, Action next)
        { 
            context.PipelineContext.Trace.Information("Message Pipeline1 task invoked!");
            next();
            context.PipelineContext.Trace.Information("Message Pipeline1 task ended!");
        }
    }
}