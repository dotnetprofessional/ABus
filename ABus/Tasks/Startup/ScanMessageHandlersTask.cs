using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ABus.Contracts;

namespace ABus.Tasks.Startup
{
    public class ScanMessageHandlersTask: IPipelineStartupTask
    {
        public IAssemblyResolver AssemblyResolver { get; set; }

        public ScanMessageHandlersTask(IAssemblyResolver assemblyResolver)
        {
            AssemblyResolver = assemblyResolver;
        }

        public async Task InvokeAsync(PipelineContext context, Func<Task> next)
        {
            var assemblies = this.AssemblyResolver.GetAssemblies();
            this.ScanForHandlers(context, assemblies, "IHandleMessage`1");
            if (context.Configuration.ReplyToQueue != null)
                this.ScanForHandlers(context, assemblies, "IHandleReplyMessage`1", context.Configuration.ReplyToQueue.Endpoint);

            await next().ConfigureAwait(false);
        }

        void ScanForHandlers(PipelineContext context, List<Assembly> assemblies, string interfaceName, string defaultSubscription= null)
        {
            var handlers = (from a in assemblies
                from t in a.GetTypes()
                // Get a list of all types within each assembly
                from i in t.GetTypeInfo().ImplementedInterfaces
                // Check TypeInfo for type 
                where i.Name == interfaceName
                //and only select those that implement IHandler(T message)
                select t).Distinct();

            foreach (var handler in handlers)
            {
                var handlerInterfaces = handler.GetTypeInfo().ImplementedInterfaces;

                // Only deal with implementations of IHandleMessage interfaces
                var interfaces = handlerInterfaces.Where(i => i.Name == interfaceName);
                foreach (var interfaceImplementation in interfaces)
                {
                    // Get the message type used
                    var argumentType = interfaceImplementation.GenericTypeArguments[0];
                    var method = interfaceImplementation.GetTypeInfo().DeclaredMethods.First();

                    // Find the already found MessageType
                    var messageType = context.RegisteredMessageTypes.FirstOrDefault(t => t.MessageType.FullName == argumentType.FullName);

                    var handlerKey = string.Format("{0}.{1}", handler.Name, messageType.MessageType.Name);
                    var registeredHandler = new RegisteredHandler(handlerKey);
                    registeredHandler.MessageType = messageType;
                    registeredHandler.Method = method;
                    registeredHandler.ClassType = handler;
                    context.RegisteredHandlers.Add(registeredHandler);

                    if (string.IsNullOrEmpty(defaultSubscription))
                        defaultSubscription = handlerKey;

                    registeredHandler.SubscriptionName = defaultSubscription;

                    context.Trace.Verbose(string.Format("Class: {0} handles {1} message type.", registeredHandler.ClassType.Name, registeredHandler.MessageType.FullName));
                }
            }
        }
    } 
}
