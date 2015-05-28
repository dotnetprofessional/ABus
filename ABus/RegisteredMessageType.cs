using System;
using System.Collections.ObjectModel;
using ABus.Contracts;

namespace ABus
{
    public class RegisteredMessageTypeCollection : KeyedCollection<string, RegisteredMessageType>
    {
        protected override string GetKeyForItem(RegisteredMessageType item)
        {
            return item.FullName;
        }
    }

    public class RegisteredMessageType
    {
        /// <summary>
        /// The full name of the type
        /// </summary>
        public string FullName { get; set; }
         
        /// <summary>
        /// The type
        /// </summary>
        public Type MessageType{ get; set; }

        /// <summary>
        /// The queue to send and recieve messages for this type on
        /// </summary>
        public string Queue { get; set; }

        /// <summary>
        /// The transport used to handle this message type
        /// </summary>
        public TransportDefinition Transport { get;set; }

        public QueueEndpoint QueueEndpoint { get { return new QueueEndpoint {Host = this.Transport.Uri, Name = this.Queue}; } }

    } 
}