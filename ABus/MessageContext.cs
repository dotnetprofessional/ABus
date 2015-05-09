using ABus.Contracts;

namespace ABus
{
    /// <summary>
    /// Contains the state of the message
    /// </summary>
    public class MessageContext
    {
        public RawMessage RawMessage { get; set; }

        public QueueEndpoint Endpoint { get; set; }

        public object TypeInstance { get; set; }

        public IBus Bus { get; set; }
    }
}