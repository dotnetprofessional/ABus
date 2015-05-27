using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    ///   but does take an instance of IGuaranteeDelivery which can be wired up via boostrapper. Commit delegates to IGuaranteeDelivery
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
    /// foreach msg in GetMessages from IGuaranteeDelivery
    ///   Send Msg - (RawMessage will have details necessary to select correct transport and intent)
    ///   Mark as complete
    /// 
    /// End msg tx (via CompleteAsync)
    /// 
    /// 
    /// Package:
    /// No special logic needed for providers
    /// SQLGuaranteedDelivery
    ///   IGuaranteeDelivery implementation
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
        void Begin();

        void Commit();

        /// <summary>
        /// Record them in a stack
        /// </summary>
        /// <param name="message"></param>
        void AddItem(RawMessage message);
    }

    /// <summary>
    /// Used in conjunction with <see cref="IManageOutboundMessages"/> implemenations
    /// of this interface ensure committed messages are delivered to the destination queue
    /// </summary>
    public interface IGuaranteeDelivery
    {
        /// <summary>
        /// Stores all outbound messages associated with the inbound message id
        /// </summary>
        /// <param name="messageId">messageId of the inbound message</param>
        /// <param name="messages">collection of outbound messages</param>
        void StoreMessages(string messageId, IEnumerable<RawMessage> messages);

        /// <summary>
        /// Returns all outbound messages associated with the messageId
        /// </summary>
        /// <param name="messageId">messageId of the inbound message</param>
        /// <returns></returns>
        IEnumerable<RawMessage> GetMessages(string messageId);

        /// <summary>
        /// Marks a message as having been successfully delivered to the destination queue
        /// </summary>
        /// <param name="messageId">messageId of the inbound message</param>
        /// <param name="childMessageId">messageId of the outbound message</param>
        void MarkAsComplete(string messageId, string childMessageId);
    }
}
