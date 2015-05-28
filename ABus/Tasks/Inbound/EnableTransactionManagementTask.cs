using System;
using ABus.Contracts;

namespace ABus.Tasks.Inbound
{
    class EnableTransactionManagementTask : IPipelineInboundMessageTask
    {
        public void Invoke(InboundMessageContext context, Action next)
        {
            var messageManager = context.PipelineContext.ServiceLocator.GetInstance<IManageOutboundMessages>();
            using (messageManager as IDisposable)
            {
                messageManager.InboundMessageId = context.RawMessage.MessageId;

                // check if we've already done this work before
                if (!messageManager.AlreadyProcessed())
                {
                    messageManager.Begin();

                    // Set the transaction manager on the context so that messages can be added by other tasks
                    context.TransactionManager = messageManager;

                    next();

                    // Persist outbound messages with any database transactions in an ACID transaction (if supported by transaciton manager)
                    messageManager.Commit();
                }

                // Now need to dispatch the outbound messages to their respective queues using the appropriate transport
                foreach (var m in messageManager.TransactionManager.GetMessages(messageManager.InboundMessageId))
                {
                    var messageTypeName = m.MetaData[StandardMetaData.MessageType].Value;
                    var messageType = context.PipelineContext.RegisteredMessageTypes[messageTypeName];
                    var transport = context.PipelineContext.TransportInstances[messageType.Transport.Name];
                    var actionType = context.RawMessage.MetaData[StandardMetaData.ActionType].Value;

                    if (actionType == OutboundMessageContext.ActionType.Send.ToString())
                        transport.Send(messageType.QueueEndpoint, context.RawMessage);
                    else if (actionType == OutboundMessageContext.ActionType.Publish.ToString())
                        transport.Publish(messageType.QueueEndpoint, context.RawMessage);
                    else if (actionType == OutboundMessageContext.ActionType.Reply.ToString())
                        transport.Send(messageType.QueueEndpoint, context.RawMessage);

                    messageManager.TransactionManager.MarkAsComplete(messageManager.InboundMessageId, m.MessageId);
                }
            }
        }
    }
}
