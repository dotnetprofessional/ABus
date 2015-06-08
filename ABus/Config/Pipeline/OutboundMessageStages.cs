namespace ABus.Config.Pipeline
{
    public class OutboundMessageStages
    {
        public const string ValidateBestPractices = "ValidateBestPractices";
        public const string CreateRawMessage = "TransformOutboundMessage";
        public const string TransformOutboundMessage = "CreateRawMessage";
        public const string Serialize = "Serialize";
        public const string TransformOutboundRawMessage = "TransformOutboundRawMessage";
        public const string SendMessage = "SendMessage";
    }
}