using System;
using System.Threading.Tasks;

namespace ABus.Contracts
{
    public interface IBus : IDisposable
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
        Task PublishAsync(object message);

        /// <summary>
        /// Sends a command message to the active transport
        /// </summary>
        /// <param name="message"></param>
        Task SendAsync(object message);

        /// <summary>
        /// Sends a reply to a command message on the active transport
        /// </summary>
        /// <param name="message"></param>
        Task ReplyAsync(object message);

        /// <summary>
        /// Forwards the current message to the error queue and removes it from the subscription queue
        /// </summary>
        void DeadLetterMessage();

        /// <summary>
        /// Terminates processing of this message by other plugins within the pipeline
        /// </summary>
        /// <remarks>
        /// This method is useful to terminate processing of messages during authentication or authorization
        /// </remarks>
        void TerminateMessagePipeline();

        ABusTraceSource Trace { get; }
    }
}