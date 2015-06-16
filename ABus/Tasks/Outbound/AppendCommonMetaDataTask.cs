using System;
using ABus.Contracts;

namespace ABus.Tasks.Outbound
{
    class AppendCommonMetaDataTask : IPipelineOutboundMessageTask
    {
        public void Invoke(OutboundMessageContext context, Action next)
        {
            context.RawMessage.MetaData.Add(new MetaData{Name = "Source", Value = Environment.MachineName});

            if (context.InboundMessageContext.Bus.CurrentMessage != null)
            {
                string currentCorrelationId = "";
                if (context.InboundMessageContext.Bus.CurrentMessage.CorrelationId != null)
                    currentCorrelationId = context.InboundMessageContext.Bus.CurrentMessage.CorrelationId;
                else
                    currentCorrelationId = Guid.NewGuid().ToString();

                context.RawMessage.CorrelationId = currentCorrelationId;
            }
            next();
        } 
    }
}
