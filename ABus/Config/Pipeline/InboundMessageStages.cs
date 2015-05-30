namespace ABus.Config.Pipeline
{
    public class InboundMessageStages
    {
        public const string Security = "Security";
        public const string TransactionManagement = "TransactionManagement";
        public const string TransformInboundRawMessage = "TransformInboundRawMessage";
        public const string Deserialize = "Deserialize";
        public const string TransformInboundMessage = "TransformInboundMessage";
        public const string ExecuteHandler = "ExecuteHandler";
    }
}