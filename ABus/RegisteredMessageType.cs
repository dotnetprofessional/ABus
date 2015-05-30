using System;
using ABus.Contracts;

namespace ABus
{
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