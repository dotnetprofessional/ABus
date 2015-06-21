using System;

namespace ABus.Contracts
{
    public class TransportDefinition
    {
        public TransportDefinition()
        {
            this.AuditQueue = "Audit";
            this.EnableAuditing = true;
        }

        /// <summary>
        /// Unique name to define the transport definition
        /// </summary>
        public string Name { get; set; } 

        /// <summary>
        /// The endpoint the transport will connect to, excluding the queue name
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// Set of credentials the transport can use to authenticate with
        /// </summary>
        public string Credentials { get; set; }

        /// <summary>
        /// A fully qualified definition of the transport class to use
        /// </summary> 
        public string TransportType { get; set; }

        /// <summary>
        /// The queue where all messages sent to any queue for this transport will be forwarded
        /// </summary>
        public string AuditQueue { get; set; }

        /// <summary>
        /// Specifies if messages should be audited
        /// </summary>
        public bool EnableAuditing { get; set; }
    }
}