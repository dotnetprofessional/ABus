namespace ABus.Contracts
{
    public static class StandardMetaData
    {
        public static readonly string MessageType = "ABus.MessageType";
        public static readonly string ContentType = "ABus.Content-Type";
        public static readonly string MessageIntent = "ABus.MessageIntent";
        public static readonly string ShouldDeadLetterMessage = "Internal.ShouldDeadLetterMessage";
        public static readonly string Exception = "ABus.Exception";
        public static readonly string AuthenticatedUser = "ABus.AuthenticatedUser";
        public static readonly string AuthenticationType = "ABus.AuthenticationType";
        public static readonly string SourceEndpoint = "ABUs.SourceEndpoint";
        public static readonly string ConversationId = "ABus.ConversationId";
        public static readonly string RelatedTo = "ABus.RelatedTo";
    } 
}
