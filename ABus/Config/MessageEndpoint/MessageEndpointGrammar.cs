namespace ABus.Config.MessageEndpoint
{
    public class MessageEndpointGrammar
    {
        internal ConfigurationGrammar Parent { get; set; }

        public MessageEndpointGrammar(ConfigurationGrammar parent)
        {
            this.Parent = parent;
        }

        public MessageEndpointDefinitionGrammar UseTransport(string name)
        {
            var definition = new MessageEndpointDefinition { TransportName = name };
            this.Parent.Configuration.MessageEndpointDefinitions.Add(definition);

            return new MessageEndpointDefinitionGrammar(this, definition);
        }
    }
}
