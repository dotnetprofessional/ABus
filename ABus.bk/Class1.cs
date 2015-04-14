using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ABus.Contracts;
using Newtonsoft.Json;

namespace ABus
{
    public class Bus  :IBus
    {
        public Bus()
        {
        }

        public RawMessage CurrentMessage
        {
            get { throw new NotImplementedException(); }
        }

        public void Publish(IEvent message)
        {
            throw new NotImplementedException();
        }

        public void Send(ICommand message)
        {
            throw new NotImplementedException();
        }

        public void Reply(ICommand message)
        {
            throw new NotImplementedException();
        }

        public void TerminateMessagePipeline()
        {
            throw new NotImplementedException();
        }


    }

    public class BusProcessHost
    {
        public Type Handler { get; set; }
        IMessageTransport Transport { get; set; }

        public BusProcessHost(Type handler, IMessageTransport transport)
        {
            var X = SearchForHandlers();
            this.Handler = handler;
            this.Transport = transport;
            this.Transport.MessageReceived += Transport_MessageReceived;

            this.RegisterEventHandlers(handler);
        }

        void Transport_MessageReceived(object sender, RawMessage e)
        {
            // Find the handler for this message
            if (this.MessageHandlers.ContainsKey(sender.ToString()))
            {
                var handler = this.MessageHandlers[sender.ToString()];

                // Convert the Raw message into the class instance
                // Assuming type is correct - need to add a header and verify
                var json = System.Text.Encoding.Unicode.GetString(e.Body, 2, e.Body.Length-2);
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

            var interfaces = handler.GetTypeInfo().ImplementedInterfaces;

            foreach (var impl in interfaces.Where(i => i.Name == "IHandleMessage`1"))
            {
                var genericType = impl.GenericTypeArguments[0];
                var method = impl.GetTypeInfo().DeclaredMethods.FirstOrDefault(m => m.Name == "Handler");
                var queue = genericType.FullName;
                var subscriptionKey = handler.FullName;

                var typeInstance = Activator.CreateInstance(handler);
                // Search for a property that supports 
                var handlerInstance = new HandlerInstance();
                handlerInstance.MessageTypeName = impl.FullName;
                handlerInstance.MessageType = genericType;
                handlerInstance.Method = method;

                handlerInstance.TypeInstance = typeInstance;

                // Now that we have a valid subscription record the handler for the subscription
                this.MessageHandlers.Add(string.Format("{0}:{1}", queue, subscriptionKey), handlerInstance);

                // Wait for the subscription to be created before going to the next
                // TODO: Might make this wait for all to complete then do a compensating action should one fail
                // Now subscribe for messages
                // At the moment commands and events are treated the same, this needs to change later
                var t = this.Transport.SubscribeAsync(queue, subscriptionKey);
                t.Wait();
            }
        }

        /// <summary>
        /// Searches all loaded dlls for classes that implement the IHandeMesssage interface
        /// </summary>
        /// <returns></returns>
        List<object> SearchForHandlers()
        {
            var installPath = Assembly.GetExecutingAssembly().l


            //IEnumerable<Type> types =
            //from a in AppDomain.CurrentDomain.GetAssemblies()
            //from t in a.GetTypes()
            //select t;
            return null;
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
