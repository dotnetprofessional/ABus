using System;

namespace ABus.Contracts
{
    /// <summary>
    /// The mechanism use to move messages from sender to reciever
    /// </summary>
    public interface IMessageQueue
    {
        /// <summary>
        /// Starts monitoring the configured queue
        /// </summary>
        /// <param name="endpoint"></param>
        void StartMonitoring(QueueEndpoint endpoint);

        /// <summary>
        /// Sends a message to the configured queue
        /// </summary>
        /// <param name="message"></param>
        void SendMessage(RawMessage message);

        /// <summary>
        /// Sends a message that will be queued in the future
        /// </summary>
        /// <param name="message"></param>
        void SendDelayedMessage(RawMessage message);

        //void Subscribe()
        event EventHandler<RawMessageReceivedEventArgs> RawMessageReceived;
    }
}