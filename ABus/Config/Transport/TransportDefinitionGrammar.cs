using System;
using ABus.Contracts;

namespace ABus.Config.Transport
{
    public class TransportDefinitionGrammar
    {
        ConfigurationGrammar Parent { get; set; }
        TransportDefinition Definition { get; set; }

        public TransportDefinitionGrammar(ConfigurationGrammar parent, TransportDefinition definition)
        {
            this.Parent = parent;
            this.Definition = definition;
        }

        public ConfigurationGrammar And()
        {
            return this.Parent;
        }
        
        public TransportDefinitionGrammar WithConnectionString(string connectionstring)
        {
            // TODO: Add more validation

            // The connection string format is a complex object so needs to be broken apart
            var parts = connectionstring.Split(';');
            if (parts.Length != 3)
                throw new ArgumentException("Transport connection string is invalid: " + connectionstring);

            this.Definition.Uri = parts[0].Substring(9);
            this.Definition.Credentials = string.Format("{0};{1}", parts[1], parts[2]);

            return this;
        }

        public TransportDefinitionGrammar UsingAuditQueue(string auditQueue)
        {
            this.Definition.AuditQueue = auditQueue;
            return this;
        }
    }
}
