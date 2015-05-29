using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ABus.Contracts
{

    /// <summary>
    /// NOTES:
    /// 
    /// With Outbox pattern
    /// AzureSQLTransaction
    /// 
    /// Without outbox pattern
    /// AzureBasicTransaction
    /// 
    /// incomming message: new AzureSQLTransaction
    /// 
    /// 
    /// Logic
    /// Unit of Work handler is generic and not specific to a persistence layer so is part of the IncommingMessageContext
    ///   but does take an instance of IManageTransactions which can be wired up via boostrapper. Commit delegates to IManageTransactions
    /// Define transaction strategy ie SQLServerTransaction
    /// dequeue msg 
    /// Start msg tx (via a peek)
    /// Start Tx
    /// if(!msg previously processed)
    ///   
    ///   Invoke Handler
    ///      Record Send/Publish/Reply via UoW
    ///   Commit UoW
    /// end if
    /// commit tx
    /// 
    /// *** No Db tx as 
    /// foreach msg in GetMessages from IManageTransactions
    ///   Send Msg - (RawMessage will have details necessary to select correct transport and intent)
    ///   Mark as complete
    /// 
    /// End msg tx (via CompleteAsync)
    /// 
    /// 
    /// Package:
    /// No special logic needed for providers
    /// SQLGuaranteedDelivery
    ///   IManageTransactions implementation
    /// 
    /// </summary>


    /// <summary>
    /// Maintains a list of messages affected by a business transaction and coordinates the writing 
    /// out of changes and the resolution of concurrency problems.
    /// </summary>
    /// <remarks> 
    /// Azure doesn't support DTC transactions, this interface provides a way to keep all transactions
    /// within the same resource context. Examples would be SQL Server used to record message transactions.
    /// </remarks>
    public interface IManageOutboundMessages
    {
        string InboundMessageId { get; set; }
        void Begin();

        void Commit();

        /// <summary>
        /// Record them in a stack
        /// </summary>
        /// <param name="message"></param>
        void AddItem(RawMessage message);

        bool AlreadyProcessed();

        IManageTransactions TransactionManager { get; }
    }

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

    /// <summary>
    /// Used in conjunction with <see cref="IManageOutboundMessages"/> implemenations
    /// of this interface ensure committed messages are delivered to the destination queue
    /// </summary>
    public interface IManageTransactions
    {
        /// <summary>
        /// Stores all outbound messages associated with the inbound message id
        /// </summary>
        /// <param name="inboundMessageId">messageId of the inbound message</param>
        /// <param name="messages">collection of outbound messages</param>
        void StoreMessages(string inboundMessageId, IEnumerable<RawMessage> messages);

        /// <summary>
        /// Returns all outbound messages associated with the messageId
        /// </summary>
        /// <param name="inboundMessageId">messageId of the inbound message</param>
        /// <returns></returns>
        IEnumerable<RawMessage> GetMessages(string inboundMessageId);

        /// <summary>
        /// Marks a message as having been successfully delivered to the destination queue
        /// </summary>
        /// <param name="messageId">messageId of the inbound message</param>
        /// <param name="childMessageId">messageId of the outbound message</param>
        void MarkAsComplete(string messageId, string childMessageId);

        bool Exists(string messageId);
    }

    /// <summary>
    /// This transaction manager provides an in memory record of which messages have been
    /// successfully sent. It is resiliant to tempory failures via retry, but is not able
    /// to guarantee delivery if the inbound message is pushed back to the queue or the process
    /// is recyled as state is not persisted. This implementation should not be used in production
    /// environments as you may lose outbound messages.
    /// </summary>
    public class DefaultTransactionManager : IManageTransactions
    {
        static Dictionary<string, Dictionary<string, RawMessage>> Repository { get; set; }
        static object syncLock = new object();

        public DefaultTransactionManager()
        {
            lock (syncLock)
            {
                if (Repository == null)
                    Repository = new Dictionary<string, Dictionary<string, RawMessage>>();                
            }
        }

        public void StoreMessages(string inboundMessageId, IEnumerable<RawMessage> messages)
        {
            lock (syncLock)
            {
                if (Repository.ContainsKey(inboundMessageId))
                    throw new ArgumentException("Duplicate messageId: " + inboundMessageId);

                Repository.Add(inboundMessageId, messages.ToDictionary(m => m.MessageId, m => m));
            }
        }

        public IEnumerable<RawMessage> GetMessages(string inboundMessageId)
        {
            if (Repository.ContainsKey(inboundMessageId))
                return Repository[inboundMessageId].Values.ToList();  

            return null;
        }

        public void MarkAsComplete(string inboundMessageId, string childMessageId)
        {
            lock (syncLock)
            {
                // Locate messages for the inbound message id
                var messages = Repository[inboundMessageId];
                // Now remove item from list
                messages.Remove(childMessageId);
            }
        }

        public bool Exists(string inboundMessageId)
        {
            return Repository.ContainsKey(inboundMessageId);
        }
    }
}
