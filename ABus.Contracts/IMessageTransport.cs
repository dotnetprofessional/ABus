using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABus.Contracts
{
    public interface IMessageTransport
    {
        MetaDataCollection Configuration { get; set; }

        QueueEndpoint Endpoint { get; set; }
        /// <summary>
        /// Publishes an event message to the Endpoint
        /// </summary>
        /// <param name="message"></param>
        void Publish(string queue, RawMessage message);

        /// <summary>
        /// Sends a command message to the Endpoint
        /// </summary>
        /// <param name="message"></param>
        void Send(string queue, RawMessage message);

        /// <summary>
        /// Create a subsription on the Endpoint
        /// </summary>
        /// <param name="name">The unique name for this subscription</param>
        void Subscribe(string queue, string name);
    }
}
