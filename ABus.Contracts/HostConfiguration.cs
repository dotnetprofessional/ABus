using System.Collections.Generic;

namespace ABus.Contracts
{
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