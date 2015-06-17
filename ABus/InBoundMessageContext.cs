using System;
using System.Collections.Generic;
using ABus.Contracts;
using Newtonsoft.Json;

namespace ABus
{
    /// <summary>
    /// Contains the state of the message
    /// </summary>
    public class InboundMessageContext
    {
        string RawMessageCopy { get; set; }
        string SourceCopy { get; set; }

        public InboundMessageContext(string source, RawMessage rawMessage, PipelineContext pipelineContext)
        {
            // Save a copy of the orginal message inputs incase a reset is required (namely during error retrys)
            this.SourceCopy = source;
            this.RawMessageCopy = JsonConvert.SerializeObject(rawMessage);

            this.ParseSource(source);
            this.RawMessage = rawMessage;
            this.PipelineContext = pipelineContext;
            this.OutboundMessages = new List<RawMessage>();
            this.ShouldTerminatePipeline = false;
        }

        void ParseSource(string source)
        {
            if (!string.IsNullOrEmpty(source))
            {
                // Source should have the format [queue]:[subscription]
                var sourceParts = source.Split(':');
                this.Queue = sourceParts[0];
                this.SubscriptionName = sourceParts[1];
            }
        }

        /// <summary>
        /// This method will reset the pipeline to the same state as it was when the message
        /// arrived. Any state changes will therefore be lost, including any changes to the RawMessage
        /// </summary>
        public void Reset()
        {
            // Initalize this instance back to the original
            this.ParseSource(this.SourceCopy);
            var originalRawMessage = JsonConvert.DeserializeObject<RawMessage>(this.RawMessageCopy);
            // Transfer properties - can't just swap it out as there are references to this instance
            this.RawMessage.Body = originalRawMessage.Body;
            this.RawMessage.MessageId = originalRawMessage.MessageId;
            this.RawMessage.MetaData = originalRawMessage.MetaData;
            this.RawMessage.CorrelationId = originalRawMessage.CorrelationId;

            this.OutboundMessages = new List<RawMessage>();
            this.ShouldTerminatePipeline = false;
        }

        public RawMessage RawMessage { get; private set; }

        public string Queue { get; set; }

        public string SubscriptionName { get; private set; }

        public object TypeInstance { get; set; }

        public PipelineContext PipelineContext { get; private set; }

        public IBus Bus { get; set; }

        public List<RawMessage> OutboundMessages { get; set; }

        public bool ShouldTerminatePipeline { get; set; }

    }
}