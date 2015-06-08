using ABus.Contracts;

namespace ABus
{
    /// <summary>
    /// Contains the state of the message
    /// </summary>
    public class OutboundMessageContext
    {
        public enum MessageIntent
        {
            Send,
            Reply,
            Publish
        }

        public OutboundMessageContext(QueueEndpoint queue, object messageInstance, PipelineContext pipelineContext, InboundMessageContext inboundMessageContext)
        {
            this.Queue = queue;
            this.MessageInstance = messageInstance;
            this.PipelineContext = pipelineContext;
            this.InboundMessageContext = inboundMessageContext;
            this.RawMessage = new RawMessage();
        }

        public RawMessage RawMessage { get; private set; }

        public QueueEndpoint Queue { get; private set; }

        public object MessageInstance { get; set; }

        public PipelineContext PipelineContext { get; private set; }

        public InboundMessageContext InboundMessageContext { get; private set; }
    }
}