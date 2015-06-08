using System.Collections.Generic;

namespace ABus.Contracts
{
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
}