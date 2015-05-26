using System;
using ABus.Contracts;

namespace ABus.Tasks.Outbound
{
    class SendMessageTask :IPipelineOutboundMessageTask
    {
        public void Invoke(OutboundMessageContext context, Action next)
        {
            var messageType = context.PipelineContext.RegisteredMessageTypes[context.MessageInstance.GetType().FullName];
            var transport = context.PipelineContext.TransportInstances[messageType.Transport.Name];
            var actionType = context.RawMessage.MetaData[StandardMetaData.ActionType].Value;

            if(actionType == OutboundMessageContext.ActionType.Send.ToString())
                transport.Send(context.Queue, context.RawMessage);
            else if (actionType == OutboundMessageContext.ActionType.Publish.ToString())
                transport.Publish(context.Queue, context.RawMessage);
            else if (actionType == OutboundMessageContext.ActionType.Reply.ToString())
                transport.Send(context.Queue, context.RawMessage);

            next();
        }
    }
}
