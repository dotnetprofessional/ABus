using ABus.Contracts;

namespace ABus
{
    public class RegisteredMessageType
    {
        /// <summary>
        /// The name of the type
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The fully qualified path for the type
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The queue to send and recieve messages for this type on
        /// </summary>
        public string Queue { get; set; }

        /// <summary>
        /// The transport used to handle this message type
        /// </summary>
        public TransportDefinition Transport { get;set; }

    } 
}