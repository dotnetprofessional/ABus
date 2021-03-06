using System;
using System.Collections.Generic;
using System.Diagnostics;
using ABus.Config;
using ABus.Contracts;
using Microsoft.Practices.ServiceLocation;

namespace ABus
{
    /// <summary>
    /// Contains the state of the Pipeline
    /// </summary>
    public class PipelineContext
    {
        public event EventHandler<RawMessage> MessageReceivedHandler;

        public PipelineContext(IServiceLocator serviceLocator, Configuration configuration, ABusTraceSource traceListener)
        {
            this.ServiceLocator = serviceLocator;
            this.Configuration = configuration;
            this.RegisteredMessageTypes = new RegisteredMessageTypeCollection();
            this.RegisteredHandlers = new RegisteredHandlerCollection();
            this.TransportInstances = new Dictionary<string, IMessageTransport>();

            // Used to output trace information
            this.Trace = traceListener;
        }

        public RegisteredMessageTypeCollection RegisteredMessageTypes { get; set; }

        public RegisteredHandlerCollection RegisteredHandlers { get; set; } 
         
        public Configuration Configuration { get; set; }

        public IServiceLocator ServiceLocator { get; private set; }

        public Dictionary<string, IMessageTransport> TransportInstances { get; set; }

        public ABusTraceSource Trace { get; private set; }

        public void RaiseMessageReceivedEvent(object sender, RawMessage e)
        {
            if (this.MessageReceivedHandler != null)
                this.MessageReceivedHandler(sender, e);
        }

        public void DispatchMessage(RawMessage m)
        {
            var messageTypeName = m.MetaData[StandardMetaData.MessageType].Value;
            var messageType = this.RegisteredMessageTypes[messageTypeName];
            var transport = this.TransportInstances[messageType.Transport.Name];
            var messageIntent = m.MetaData[StandardMetaData.MessageIntent].Value;

            if (messageIntent == OutboundMessageContext.MessageIntent.Send.ToString())
                transport.Send(messageType.QueueEndpoint, m);
            else if (messageIntent == OutboundMessageContext.MessageIntent.Publish.ToString())
                transport.Publish(messageType.QueueEndpoint, m);
            else if (messageIntent == OutboundMessageContext.MessageIntent.Reply.ToString())
                transport.Send(messageType.QueueEndpoint, m);

            this.Trace.Verbose(string.Format("Dispatched message {0}", m.MessageId));
        }

    }
}