using System;
using ABus.Contracts;

namespace ABus.Tasks.Outbound
{
    class AppendCommonMetaDataTask : IPipelineOutboundMessageTask
    {
        public void Invoke(OutboundMessageContext context, Action next)
        {
            context.RawMessage.MetaData.Add(new MetaData{Name = "Source", Value = Environment.MachineName});

            next();
        }
    }
}
