using System;
using System.Collections.Generic;
using ABus.Contracts;

namespace ABus.Tasks.Inbound
{
    internal class DispatchOuboundMessagesTask : IPipelineInboundMessageTask
    {
        public void Invoke(InboundMessageContext context, Action next)
        {
            // This task happens at the end of the inbound pipeline which includes all
            // calls to the outbound pipeline for any messages that have been sent.
            // This task however must be done once ALL messages have been queued for sending
            // which is why it can't be part of the outbound pipeline

            next();

            var transactionsEnabled = context.PipelineContext.Configuration.Transactions.TransactionsEnabled;

            IEnumerable<RawMessage> outboundMessages = null;
            var messageManager = context.PipelineContext.ServiceLocator.GetInstance<OutboundMessageManager>();

            if (transactionsEnabled)
                outboundMessages = messageManager.TransactionManager.GetMessages(context.RawMessage.MessageId);
            else
                outboundMessages = context.OutboundMessages;

            // Now need to dispatch the outbound messages to their respective queues using the appropriate transport
            foreach (var m in outboundMessages)
            {
                context.PipelineContext.DispatchMessage(m);

                if (messageManager != null && transactionsEnabled)
                    messageManager.TransactionManager.MarkAsComplete(context.RawMessage.MessageId, m.MessageId);
            }
        }
    }
}