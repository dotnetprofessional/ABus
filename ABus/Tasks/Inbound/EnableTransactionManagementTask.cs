using System;
using ABus.Contracts;
using System.Transactions;

namespace ABus.Tasks.Inbound
{
    /// <summary>
    /// Provides implementation of the outbox pattern
    /// </summary>
    /// <remarks>
    /// Based on these references:
    /// http://blogs.msdn.com/b/clemensv/archive/2012/07/30/transactions-in-windows-azure-with-service-bus-an-email-discussion.aspx
    /// http://gistlabs.com/2014/05/the-outbox/
    /// http://www.eaipatterns.com/GuaranteedMessaging.html
    /// http://docs.particular.net/nservicebus/outbox/
    /// </remarks>
    class EnableTransactionManagementTask : IPipelineInboundMessageTask
    { 
        public void Invoke(InboundMessageContext context, Action next)
        {
            if (!context.PipelineContext.Configuration.Transactions.TransactionsEnabled)
            {
                context.PipelineContext.Trace.Warning("Transaction support disabled!");
                // Effectively by-pass this task and go to the next one.
                next();
                
                return;
            }

            var messageManager = context.PipelineContext.ServiceLocator.GetInstance<OutboundMessageManager>();
            using (messageManager)
            {
                messageManager.InboundMessageId = context.RawMessage.MessageId;

                // check if we've already done this work before
                if (!messageManager.AlreadyProcessed())
                {
                    messageManager.Begin();

                    next();

                    // Transfer all outbound messages to the transaction manager
                    messageManager.AddRangeItems(context.OutboundMessages);

                    // Persist outbound messages with any database transactions in an ACID transaction (if supported by transaciton manager)
                    messageManager.Commit();
                }
            }

            // Now need to dispatch the outbound messages to their respective queues using the appropriate transport
            foreach (var m in messageManager.TransactionManager.GetMessages(messageManager.InboundMessageId))
            {
               
                var messageTypeName = m.MetaData[StandardMetaData.MessageType].Value;
                var messageType = context.PipelineContext.RegisteredMessageTypes[messageTypeName];
                var transport = context.PipelineContext.TransportInstances[messageType.Transport.Name];
                var messageIntent = m.MetaData[StandardMetaData.MessageIntent].Value;

                if (messageIntent == OutboundMessageContext.MessageIntent.Send.ToString())
                    transport.Send(messageType.QueueEndpoint, m);
                else if (messageIntent == OutboundMessageContext.MessageIntent.Publish.ToString())
                    transport.Publish(messageType.QueueEndpoint, m);
                else if (messageIntent == OutboundMessageContext.MessageIntent.Reply.ToString())
                    transport.Send(messageType.QueueEndpoint, m);

                messageManager.TransactionManager.MarkAsComplete(messageManager.InboundMessageId, m.MessageId); 
                
            }
        }
    }
}
