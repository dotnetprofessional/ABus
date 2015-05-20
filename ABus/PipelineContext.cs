using System.Collections.Generic;
using ABus.Contracts;
using Microsoft.Practices.ServiceLocation;

namespace ABus
{
    /// <summary>
    /// Contains the state of the Pipeline
    /// </summary>
    public class PipelineContext
    {
        public PipelineContext(IServiceLocator serviceLocator)
        {
            this.ServiceLocator = serviceLocator;
            this.AvailableTransports = new List<TransportDefinition>();
            this.RegisteredMessageTypes = new List<RegisteredMessageType>();
            this.RegisteredHandlers = new List<RegisteredHandler>();
            this.Configuration = new PipelineConfiguration();
            this.TransportInstances = new Dictionary<string, IMessageTransport>();
        }

        public List<TransportDefinition> AvailableTransports { get; set; } 

        public List<RegisteredMessageType> RegisteredMessageTypes { get; set; }

        public List<RegisteredHandler> RegisteredHandlers { get; set; } 

        public PipelineConfiguration Configuration { get; set; }

        public IServiceLocator ServiceLocator { get; private set; }

        public Dictionary<string, IMessageTransport> TransportInstances { get; set; } 
    }
}