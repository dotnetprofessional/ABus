using System;

namespace ABus.Contracts
{
    public class TransportDefinition
    {
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

        [Obsolete("Handled elsewhere - delete once transports have been initialized.")]
        public Type TransportObsolete { get; set; }

        public Transport Transport { get; set; }
    }
}