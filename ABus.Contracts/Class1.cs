using System;

namespace ABus.Contracts
{
    public class HostDefinition
    {
        public string Uri { get; set; }

        public string Credentials { get; set; }

        public Type Transport { get; set; }
    }
}
