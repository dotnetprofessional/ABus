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
                if (context.InboundMessageContext.Bus.CurrentMessage.MetaData.Contains(StandardMetaData.CorrelationId))
                    currentCorrelationId = context.InboundMessageContext.Bus.CurrentMessage.MetaData[StandardMetaData.CorrelationId].Value;
                else
                    currentCorrelationId = Guid.NewGuid().ToString();

                context.RawMessage.MetaData.Add(new MetaData { Name = StandardMetaData.CorrelationId, Value = currentCorrelationId });
            }
            next();
        } 
    }
}
