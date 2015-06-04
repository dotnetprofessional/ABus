using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ABus.Config;
using ABus.Contracts;
using Microsoft.Practices.ServiceLocation;

namespace ABus
{
    public class Pipeline
    {
        IServiceLocator ServiceLocator { get; set; }

        public ABusTraceSource Trace { get; set; }
        PipelineContext PipelineContext { get; set; }

        //BlockingCollection<IPipelineTask> Tasks;

        public Pipeline()
        {
            this.Trace = new ABusTraceSource();
            this.Configure = new ConfigurationGrammar();

            //this.Tasks = new BlockingCollection<IPipelineTask>();
        }

        public ConfigurationGrammar Configure { get; private set; }
        /// <summary>
        /// Start processing the pipeline
        /// </summary>
        public void Start()
        {
            if(this.Configure.Configuration.Container == null)
                throw new ArgumentException("You must specify an IoC container such as Unity as part of the configuration.");

            this.ServiceLocator = this.Configure.Configuration.Container;

            this.PipelineContext = new PipelineContext(this.ServiceLocator, this.Configure.Configuration, this.Trace);
            this.PipelineContext.MessageReceivedHandler += InboundMessageReceived;

            var tasks = this.Configure.Configuration.Pipeline.StartupPipelineTasks.GetTasks();
            if(tasks.Count > 0)
                this.ExecuteStartupTask(this.PipelineContext, tasks.First);
        }

        /// <summary>
        /// Start the pipeline and configure by searching for an implementation of IConfigureHost
        /// </summary>
        public static IBus StartUsingConfigureHost()
        {
            var p = new Pipeline();
            p.Trace.Verbose("Initializing pipeline");
            // Locate class to call
            p.ExecuteIConfigureHostIfAvailable();

            p.Start();

            return p.GetDefaultBusInstance();
        }


        IBus GetDefaultBusInstance()
        {
            var inboundMessageContext = new InboundMessageContext("", new RawMessage(), this.PipelineContext);
            var bus = new Bus(inboundMessageContext, this);
            inboundMessageContext.Bus = bus;
            return bus;
        }
        void ExecuteIConfigureHostIfAvailable()
        {
            // TODO: Had to hardcode Assembly resolver here to avoid having to specify the IoC container
            //       Need to think about how the AssemblyResolver should be leveraged.

            //var assemblyResolver = this.ServiceLocator.GetInstance<IAssemblyResolver>();
            var assemblyResolver = new AssemblyResolver();
            var assemblies = assemblyResolver.GetAssemblies();

            var hostConfigHandlers = (from a in assemblies
                from t in a.GetTypes()
                where !a.IsDynamic
                // Get a list of all types within each assembly
                from i in t.GetTypeInfo().ImplementedInterfaces
                // Check TypeInfo for type 
                where i.Name == "IConfigureHost"
                //and only select those that implement IHandler(T message)
                select t).Distinct().ToList();

            //var hostConfigHandlers = assemblies.SelectMany(assembly => assembly.GetTypes().Where(
            //    t => typeof (IConfigureHost).IsAssignableFrom(t)
            //         && !t.IsAbstract)).ToList();


            if (hostConfigHandlers.Count > 1)
                throw new ArgumentException("IConfigureHost may only be specified once per host.");

            if (hostConfigHandlers.Count == 0)
                throw new ArgumentException("Unable to locate implementation of IConfigureHost.");

            var handler = hostConfigHandlers[0];
            var handlerInterfaces = handler.GetTypeInfo().ImplementedInterfaces.Where(i => i.Name == "IConfigureHost");
            var interfaceImplementation = handlerInterfaces.FirstOrDefault();
            var method = interfaceImplementation.GetTypeInfo().DeclaredMethods.First();

            // Need to create a new instance of the class that has the handler
            var typeObject = Activator.CreateInstance(handler);
            //var typeInstance = (IConfigureHost) typeObject;

            method.Invoke(typeObject, new object[] {this.Configure});
        }


        void InboundMessageReceived(object sender, RawMessage e)
        {
            // Initialize the message context
            var inboundMessageContext = new InboundMessageContext(sender as string, e, this.PipelineContext);
            var bus = new Bus(inboundMessageContext, this);
            inboundMessageContext.Bus = bus;

            // Start the inbound message pipeline
            var tasks = this.Configure.Configuration.Pipeline.InboundMessagePipelineTasks.GetTasks();
            if (tasks.Count > 0)
                this.ExecuteInboundMessageTask(inboundMessageContext, tasks.First);
        }


        
        public void SendOutboundMessage(InboundMessageContext inboundMessageContext, OutboundMessageContext.MessageIntent messageIntent, object messageInstance)
        {
            // Initialize the message context
            var registeredType = this.PipelineContext.RegisteredMessageTypes[messageInstance.GetType().FullName];

            var outBoundMessageContext = new OutboundMessageContext(new QueueEndpoint { Host = registeredType.Transport.Uri, Name = registeredType.Queue }, messageInstance, this.PipelineContext, inboundMessageContext);
            // Record the messageIntent type
            outBoundMessageContext.RawMessage.MetaData.Add(new MetaData { Name = StandardMetaData.MessageIntent, Value = messageIntent.ToString() });

            // Start the inbound message pipeline
            var tasks = this.Configure.Configuration.Pipeline.OutboundMessagePipelineTasks.GetTasks();
            if (tasks.Count > 0)
                this.ExecuteOutboundMessageTask(outBoundMessageContext, tasks.First);
        }


        
        void ExecuteStartupTask(PipelineContext context, LinkedListNode<PipelineTask> task)
        {

            var taskInstance = this.ServiceLocator.GetInstance(task.Value.Task) as IPipelineStartupTask;
            taskInstance.Invoke(context, () =>
            {
                if (task.Next != null)
                    this.ExecuteStartupTask(context, task.Next);
            });
        }

        void ExecuteInboundMessageTask(InboundMessageContext context, LinkedListNode<PipelineTask> task)
        {

            var taskInstance = this.ServiceLocator.GetInstance(task.Value.Task) as IPipelineInboundMessageTask;
            taskInstance.Invoke(context, () =>
            {
                if (task.Next != null && !context.ShouldTerminatePipeline)
                    this.ExecuteInboundMessageTask(context, task.Next);
            });
        }
        void ExecuteOutboundMessageTask(OutboundMessageContext context, LinkedListNode<PipelineTask> task)
        {

            var taskInstance = this.ServiceLocator.GetInstance(task.Value.Task) as IPipelineOutboundMessageTask;
            taskInstance.Invoke(context, () =>
            {
                if (task.Next != null && !context.InboundMessageContext.ShouldTerminatePipeline) 
                    this.ExecuteOutboundMessageTask(context, task.Next);
            });
        }
    }
}
