namespace ABus.Contracts
{
    public interface IBus
    {

        /// <summary>
        /// Exposes the raw message taken off the queue
        /// </summary>
        /// <remarks>
        /// This can be useful if there is a need to access the message meta data
        /// </remarks>
        RawMessage CurrentMessage { get; }

        /// <summary>
        /// Publishes an event message to the active transport
        /// </summary>
        /// <param name="message"></param>
        void Publish(IEvent message);

        /// <summary>
        /// Sends a command message to the active transport
        /// </summary>
        /// <param name="message"></param>
        void Send(ICommand message);

        /// <summary>
        /// Sends a reply to a command message on the active transport
        /// </summary>
        /// <param name="message"></param>
        void Reply(ICommand message);

        /// <summary>
        /// Terminates processing of this message by other plugins within the pipeline
        /// </summary>
        /// <remarks>
        /// This method is useful to terminate processing of messages during authentication or authorization
        /// </remarks>
        void TerminateMessagePipeline();
    }
}