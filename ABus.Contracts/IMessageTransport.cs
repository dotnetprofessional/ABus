using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ABus.Contracts
{
    public interface IMessageTransport
    {
        event EventHandler<RawMessage> MessageReceived;

        void ConfigureHost(TransportDefinition transport);

        /// <summary>
        /// Publishes an event message to the Endpoint
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="message"></param>
        void Publish(QueueEndpoint endpoint, RawMessage message);

        /// <summary>
        /// Sends a command message to the Endpoint
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="message"></param>
        void Send(QueueEndpoint endpoint, RawMessage message);

        void Send(QueueEndpoint endpoint, IEnumerable<RawMessage> message);

        /// <summary>
        /// Sends a command message to the Endpoint
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="message"></param> 
        Task SendAsync(QueueEndpoint endpoint, RawMessage message);

        /// <summary>
        /// Sends a command message to the Endpoint
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="message"></param>
        Task SendAsync(QueueEndpoint endpoint, IEnumerable<RawMessage> message);

        /// <summary>
        /// Create a subsription on the Endpoint
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="subscriptionName">The unique name for this subscription</param>
        Task SubscribeAsync(QueueEndpoint endpoint, string subscriptionName);

        /// <summary>
        /// Create a queue with the supplied name
        /// </summary>
        /// <returns></returns>
        Task CreateQueueAsync(QueueEndpoint endpoint);

        /// <summary>
        /// Delete a queue with the supplied name
        /// </summary>
        /// <returns></returns>
        Task DeleteQueueAsync(QueueEndpoint endpoint);

        /// <summary>
        /// Determines if a particular queue exists
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        Task<bool> QueueExistsAsync(QueueEndpoint endpoint);

        /// <summary>
        /// Sends a message that will be queued in the future
        /// </summary>
        /// <param name="message"></param>
        /// <param name="timeToDelay">The amount of time to delay the delivery of this message</param>
        Task DeferAsync(RawMessage message, TimeSpan timeToDelay);
    }
}
