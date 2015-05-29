using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ABus.AzureServiceBus;
using ABus.Contracts;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;

namespace ABus
{
    public class Bus  :IBus
    {
        InboundMessageContext Context { get; set; }
        Pipeline Pipeline { get; set; }

        public Bus(InboundMessageContext context, Pipeline pipeline)
        {
            this.Context = context;
            this.Pipeline = pipeline;
        }

        public RawMessage CurrentMessage
        {
            get { return this.Context.RawMessage; }
        }

        public void Publish(object message)
        {
            this.Pipeline.SendOutboundMessage(this.Context, OutboundMessageContext.MessageIntent.Publish, message);
        }
         

        public void Send(object message)
        {
            this.Pipeline.SendOutboundMessage(this.Context, OutboundMessageContext.MessageIntent.Send, message);
        }

        public void Reply(object message)
        {
            this.Pipeline.SendOutboundMessage(this.Context, OutboundMessageContext.MessageIntent.Reply, message);
        }

        public void TerminateMessagePipeline()
        {
            throw new NotImplementedException();
        }


    }

    public class BusProcessHost
    {
        IUnityContainer Container { get; set; }

        MessageTransportFactory TransportFactory;
        public IMessageTransport Transport { get; set; }

        Dictionary<string, TransportDefinition> HostDefinitions;
 
        public BusProcessHost()
        {
            this.Container = new UnityContainer();


            this.InitializeTransportDefinitions();
            this.ConfigureHosts();

            this.InitializeTopics().Wait();

            var definedHandlers = SearchForHandlers();

            foreach(var h in definedHandlers)
                this.RegisterEventHandlers(h);
        }

        void RegisterTypes()
        {
            this.Container.RegisterType<IBus, Bus>();

        }
        /// <summary>
        /// Sets the transport definitions for each transport Uri
        /// </summary>
        void InitializeTransportDefinitions()
        {
            this.HostDefinitions = new Dictionary<string, TransportDefinition>();

            // TODO: Obtain this from configuration
            var host = new ABus.Contracts.TransportDefinition
            {
                Uri = "sb://abus-dev.servicebus.windows.net",
                Credentials = "SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=uyauQw6sme25rx0EzLc/2VSWafIF6PROzdkZ9A4N918=",
                TransportObsolete = typeof(AzureBusTransport)
            };

            this.HostDefinitions.Add(host.Uri, host);
        }

        void ConfigureHosts()
        {
            this.TransportFactory = new MessageTransportFactory();
            foreach (var h in this.HostDefinitions)
            {
                var transport = this.TransportFactory.GetTransport(h.Value);
                transport.MessageReceived += Transport_MessageReceived;

                // TODO: Need to find a way to support multiple transports
                this.Transport = transport;
            }
        }

        void Transport_MessageReceived(object sender, RawMessage e)
        {
            // Find the handler for this message
            if (this.MessageHandlers.ContainsKey(sender.ToString()))
            {
                var handler = this.MessageHandlers[sender.ToString()];

                // Convert the Raw message into the class instance
                // Assuming type is correct - need to add a header and verify
                var json = System.Text.Encoding.Unicode.GetString(e.Body, 0, e.Body.Length);
                var msg = JsonConvert.DeserializeObject(json, handler.MessageType);
                // Need to create a new instance of the class that has the handler
                handler.Method.Invoke(handler.TypeInstance, new object[] {msg});

            }
        }
        public void Start() { }
        public void Stop() { }

        Dictionary<string, HandlerInstance> MessageHandlers { get; set; }

        void RegisterEventHandlers(Type handler)
        {
            this.MessageHandlers = new Dictionary<string, HandlerInstance>();

            // Gets each instance of the interface that was implemented
            var handlerInterfaces = handler.GetTypeInfo().ImplementedInterfaces;

            // Only deal with implementations of IHandleMessage interfaces
            var interfaces = handlerInterfaces.Where(i => i.Name == "IHandleMessage`1");
            var tasks = new List<Task>();
            foreach (var interfaceImplementation in interfaces)
            {
                // Get the message type used
                var messageType = interfaceImplementation.GenericTypeArguments[0];

                // As the interface only has a single method we can get the first method of the interface implementation
                var method = interfaceImplementation.GetTypeInfo().DeclaredMethods.First();

                // Define the queue and subscription names to be used based on the message type and handler
                var queueName = messageType.FullName;
                var subscriptionKey = handler.FullName;

                var typeInstance = Activator.CreateInstance(handler);
                // Search for a property that supports 
                var handlerInstance = new HandlerInstance();
                handlerInstance.MessageTypeName = interfaceImplementation.FullName;
                handlerInstance.MessageType = messageType;
                handlerInstance.Method = method;

                handlerInstance.TypeInstance = typeInstance;

                // Now that we have a valid subscription record the handler for the subscription
                this.MessageHandlers.Add(string.Format("{0}:{1}", queueName, subscriptionKey), handlerInstance);

                // Wait for the subscription to be created before going to the next
                // TODO: Might make this wait for all to complete then do a compensating action should one fail
                // Now subscribe for messages
                // At the moment commands and events are treated the same, this needs to change later

                var endpoint = this.GetQueueEndpointFromType(messageType);
                Trace.Write(string.Format("Adding subscription {0}:{1}-->{2}...", endpoint.Host, endpoint.Name, subscriptionKey));
                var t = this.Transport.SubscribeAsync(endpoint, subscriptionKey);
                tasks.Add(t);
                Trace.WriteLine("done.");
            }
            Task.WaitAll(tasks.ToArray());
        }
         

        /// <summary>
        /// Searches all loaded dlls for classes that implement the IHandeMesssage interface
        /// </summary>
        /// <returns></returns>
        List<Type> SearchForHandlers()
        {
            IEnumerable<Type> types =
                (from a in AppDomain.CurrentDomain.GetAssemblies()
                 where a.FullName.StartsWith("Microsoft") == false
                    from t in a.GetTypes()
                    
                    from i in t.GetTypeInfo().ImplementedInterfaces
                    where i.Name == "IHandleMessage`1"
                    select t).Distinct();
            return types.ToList();
        }

        /// <summary>
        /// Scans all handlers and ensures that topics have been created
        /// </summary>
        /// <param name="verifyOnly">when true will only report status and not create any missing topics</param>
        public async Task InitializeTopics(bool verifyOnly = false)
        {
            var messageTypes = (from h in this.SearchForHandlers()
                from i in h.GetTypeInfo().ImplementedInterfaces
                where i.Name == "IHandleMessage`1"
                select  i.GenericTypeArguments[0]).Distinct();

            foreach (var m in messageTypes)
            {
                var queue = this.GetQueueEndpointFromType(m);
                if(await this.Transport.QueueExists(queue))
                    Trace.WriteLine(string.Format("Queue: {0} exists.", queue.Name));
                else
                {
                    Trace.Write(string.Format("Queue: {0} creating...", queue.Name));
                    await this.Transport.CreateQueue(queue);
                    Trace.WriteLine("complete.");
                }
            }

        }

        QueueEndpoint GetQueueEndpointFromType(Type messageType)
        {
            // TODO: Need to pick transport based on suppored message types
            // Only one transport is currently supported
            var endpoint = new QueueEndpoint {Host = this.HostDefinitions.First().Value.Uri, Name = messageType.FullName};
            return endpoint;
        }

    }

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

    public class HandlerInstance
    {
        public string MessageTypeName { get; set; }
        public MethodInfo Method { get; set; }

        /// <summary>
        /// Holds a reference to the method handler instance
        /// </summary>
        /// <remarks>
        /// A weak reference is used to ensure memory leaks are prevented.
        /// </remarks>
        public object TypeInstance { get; set; }

        public Type MessageType { get; set; }
    }
}
