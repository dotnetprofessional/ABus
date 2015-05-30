using System;
using System.Collections.Generic;
using ABus.Contracts;

namespace ABus
{
    public class MessageTransportFactory
    {
        Dictionary<string, IMessageTransport> HostInstances = new Dictionary<string, IMessageTransport>(); 
        public IMessageTransport GetTransport(TransportDefinition transport)
        {
            if (!this.HostInstances.ContainsKey(transport.TransportObsolete.FullName))
            {
                var hostInstance = Activator.CreateInstance(transport.TransportObsolete) as IMessageTransport;
                if (hostInstance != null)
                {
                    hostInstance.ConfigureHost(transport);
                    this.HostInstances.Add(transport.TransportObsolete.FullName, hostInstance);
                }
            }

            return this.HostInstances[transport.TransportObsolete.FullName];
        }
    }
}