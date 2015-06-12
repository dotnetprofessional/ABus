using System;
using System.Collections.Generic;
using System.Linq;
using ABus.Contracts;

namespace ABus
{
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

                try
                {
                    Repository.Add(inboundMessageId, messages.ToDictionary(m => m.MessageId, m => m));
                }
                catch (Exception)
                {
                    // PUtting this here to identify a rare bug that reprots a duplicate key
                    throw;
                }
            }
        }

        public IEnumerable<RawMessage> GetMessages(string inboundMessageId)
        {
            if (inboundMessageId != null)
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