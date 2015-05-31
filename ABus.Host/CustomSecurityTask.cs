using System;

namespace ABus.Host
{
    public class CustomSecurityTask: IPipelineInboundMessageTask
    {
        public void Invoke(InboundMessageContext context, Action next)
        {
            context.PipelineContext.Trace.Information("Authenticated request");
            next();
        }
    }
}