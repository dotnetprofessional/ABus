using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    internal class EnableTransactionManagementTask : IPipelineInboundMessageTask
    {
        public async Task InvokeAsync(InboundMessageContext context, Func<Task> next)
        {
            var transactionsEnabled = context.PipelineContext.Configuration.Transactions.TransactionsEnabled;
            IEnumerable<RawMessage> outboundMessages = null;
            OutboundMessageManager messageManager = null;

            if (!transactionsEnabled)
            {
                await next().ConfigureAwait(false);
            }
            else
            {
                // If using transactions then record the oubound messages before processing them
                // Also the transaction manager will start a new TransactionScope ambient transaction
                messageManager = context.PipelineContext.ServiceLocator.GetInstance<OutboundMessageManager>();
                using (messageManager)
                {
                    messageManager.InboundMessageId = context.RawMessage.MessageId;

                    // check if we've already done this work before
                    if (!messageManager.AlreadyProcessed())
                    {
                        messageManager.Begin();

                        await next().ConfigureAwait(false);

                        // Transfer all outbound messages to the transaction manager
                        messageManager.AddRangeItems(context.OutboundMessages);

                        // Persist outbound messages with any database transactions in an ACID transaction (if supported by transaciton manager)
                        messageManager.Commit();
                    }
                }
            }
        }
    }
}
