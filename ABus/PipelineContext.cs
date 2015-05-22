using System;
using System.Collections.Generic;
using System.Diagnostics;
using ABus.Contracts;
using Microsoft.Practices.ServiceLocation;

namespace ABus
{
    /// <summary>
    /// Contains the state of the Pipeline
    /// </summary>
    public class PipelineContext
    {
        public PipelineContext(IServiceLocator serviceLocator, ABusTraceSource traceListener)
        {
            this.ServiceLocator = serviceLocator;
            this.AvailableTransports = new List<TransportDefinition>();
            this.RegisteredMessageTypes = new RegisteredMessageTypeCollection();
            this.RegisteredHandlers = new RegisteredHandlerCollection();
            this.Configuration = new PipelineConfiguration();
            this.TransportInstances = new Dictionary<string, IMessageTransport>();

            // Used to output trace information
            this.Trace = traceListener;
        }

        public List<TransportDefinition> AvailableTransports { get; set; }

        public RegisteredMessageTypeCollection RegisteredMessageTypes { get; set; }

        public RegisteredHandlerCollection RegisteredHandlers { get; set; } 
         
        public PipelineConfiguration Configuration { get; set; }

        public IServiceLocator ServiceLocator { get; private set; }

        public Dictionary<string, IMessageTransport> TransportInstances { get; set; }

        public ABusTraceSource Trace { get; private set; }

        public event EventHandler<RawMessage> MessageReceivedHandler;
         
        public void RaiseMessageReceivedEvent(object sender, RawMessage e)
        {
            if (this.MessageReceivedHandler != null)
                this.MessageReceivedHandler(sender, e);
        }
    }
}