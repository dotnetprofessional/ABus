namespace ABus.Config.MessageEndpoint
{
    public class MessageEndpointDefinitionGrammar
    {
        MessageEndpointGrammar Parent { get; set; }
        MessageEndpointDefinition Definition { get; set; }

        public MessageEndpointDefinitionGrammar(MessageEndpointGrammar parent, MessageEndpointDefinition definition)
        {
            this.Parent = parent;
            this.Definition = definition;
        }

        public MessageEndpointDefinitionGrammar WithPattern(string pattern)
        {
            this.Definition.TypePattern = pattern;

            return this;
        }

        public MessageEndpointDefinitionGrammar WithDefaultPattern()
        {
            this.Definition.TypePattern = "default";

            return this;
        }

        public MessageEndpointDefinitionGrammar WithEndpoint(string endpoint)
        {
            this.Definition.Endpoint = endpoint;

            return this;
        }

        public MessageEndpointGrammar AndAlso { get { return this.Parent; } }

        public ConfigurationGrammar And()
        {
            return this.Parent.Parent;
        }
    }
}