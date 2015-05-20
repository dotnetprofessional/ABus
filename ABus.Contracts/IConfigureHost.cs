using System;
using System.Collections.Generic;

namespace ABus.Contracts
{
    /// <summary>
    /// Used to configure the ABus host
    /// </summary>
    public interface IConfigureHost
    {
        void SpecifyConfiguration(HostConfigurationGrammar configuration);
    }

    public class HostConfigurationGrammar
    {
        List<HostConfiguration> Configuration = new List<HostConfiguration>();

        HostConfiguration CurrentHost;
        public HostConfigurationGrammar WithHost(string uri, string credentials, Type transport)
        {
            var c = new HostConfiguration();
            c.Definition = new TransportDefinition {Uri = uri, Credentials = credentials, TransportObsolete = transport};
            this.Configuration.Add(c);
            this.CurrentHost = c;
            return this;
        }

        public HostConfigurationGrammar ProcessMessagesStartingWith(string namespacePrefix)
        {
            if (this.CurrentHost == null)
                throw new ArgumentException("You must specify the host using .WithHost before using .ProcessMessagesStartingWith");

            this.CurrentHost.Namespaces.Add(namespacePrefix);
            return this;
        }
    }

    public class HostConfiguration
    {
        public HostConfiguration()
        {
            this.Namespaces = new List<string>();
        }

        public TransportDefinition Definition { get; set; }

        public List<string> Namespaces { get; set; }
    }

}
