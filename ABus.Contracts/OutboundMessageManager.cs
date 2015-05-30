using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ABus.Contracts
{
    public class OutboundMessageManager : IManageOutboundMessages, IDisposable
    {
        public string InboundMessageId { get; set; }

        public IManageTransactions TransactionManager { get; set; }

        TransactionScope Tx { get; set; }
        List<RawMessage> Messages { get; set; }

        public OutboundMessageManager(IManageTransactions transactionManager)
        {
            this.TransactionManager = transactionManager;
        }

        public void Begin()
        {
            this.Messages = new List<RawMessage>();
            this.Tx = new TransactionScope();
        }

        public void Commit()
        {
            // Allow transaction manager to record the messages if required
            this.TransactionManager.StoreMessages(this.InboundMessageId, this.Messages);
            this.Tx.Complete();
        }

        public void AddItem(RawMessage message)
        {
            this.Messages.Add(message);
        }

        public void AddRangeItems(IEnumerable<RawMessage> messages)
        {
            this.Messages.AddRange(messages);
        }

        public bool AlreadyProcessed()
        {
            return this.TransactionManager.Exists(this.InboundMessageId);
        }

        public void Dispose()
        {
            var transactionScope = this.Tx;
            if (transactionScope != null) transactionScope.Dispose();
        }
    }
}
