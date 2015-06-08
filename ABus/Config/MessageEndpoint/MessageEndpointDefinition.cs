namespace ABus.Config.MessageEndpoint
{
    public class MessageEndpointDefinition
    {
        /// <summary>
        /// The name used when defining the transport
        /// </summary>
        public string TransportName { get; set; }

        /// <summary>
        /// This specifies the pattern to use against each message type to determine if this definition should be used.
        /// </summary>
        public string TypePattern { get; set; }

        /// <summary>
        /// Allows the specification of a specific endpoint for all message types matching the <see cref="TypePattern"/>.
        /// </summary>
        /// <remarks>
        /// If left blank the endpoint will be determined by full name of the message type.
        /// </remarks>
        public string Endpoint { get; set; }
    }
}
