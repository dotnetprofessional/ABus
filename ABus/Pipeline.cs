using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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

        public void Stop()
        {
            // Need to determine if any resources need to be shut down gracefully here.
        }

        /// <summary>
        /// Start the pipeline and configure by searching for an implementation of IConfigureHost
        /// </summary>
        public IBus StartUsingConfigureHost()
        {
            this.Trace.Verbose("Initializing pipeline");
            // Locate class to call
            this.ExecuteIConfigureHostIfAvailable();

            this.Start();

            return this.GetDefaultBusInstance();
        }


        IBus GetDefaultBusInstance()
        {
            var inboundMessageContext = new InboundMessageContext("", new RawMessage(), this.PipelineContext);
            var bus = new Bus(inboundMessageContext, this);
            inboundMessageContext.Bus = bus;
            return bus;
        }

        /// <summary>
        /// This method would ideally be part of the pipeline however as this is the first
        /// thing that must happen to configure the pipeline it can't itself be part of the pipeline
        /// </summary>
        void ExecuteIConfigureHostIfAvailable()
        {
            var hostConfigHandlers = GetTypesImplementingInterface(typeof(IConfigureHost));

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

        static List<Type> GetTypesImplementingInterface(Type type)
        {
            // TODO: Had to hardcode Assembly resolver here to avoid having to specify the IoC container
            //       Need to think about how the AssemblyResolver should be leveraged.

            //var assemblyResolver = this.ServiceLocator.GetInstance<IAssemblyResolver>();
            var assemblyResolver = new AssemblyResolver();
            var assemblies = assemblyResolver.GetAssemblies();

            var typesFound = assemblies.SelectMany(assembly => assembly.GetTypes().Where(
                t => type.IsAssignableFrom(t)
                     && !t.IsAbstract)).ToList();

            return typesFound;
        }


        async void InboundMessageReceived(object sender, RawMessage e)
        {
            // Initialize the message context
            var inboundMessageContext = new InboundMessageContext(sender as string, e, this.PipelineContext);
            var bus = new Bus(inboundMessageContext, this);
            inboundMessageContext.Bus = bus;

            // Start the inbound message pipeline
            var tasks = this.Configure.Configuration.Pipeline.InboundMessagePipelineTasks.GetTasks();
            if (tasks.Count > 0)
                await this.ExecuteInboundMessageTask(inboundMessageContext, tasks.First);
        }


        
        public async Task SendOutboundMessage(InboundMessageContext inboundMessageContext, MessageIntent messageIntent, object messageInstance)
        {
            // Initialize the message context
            var registeredType = this.PipelineContext.RegisteredMessageTypes[messageInstance.GetType().FullName];

            var outBoundMessageContext = new OutboundMessageContext(new QueueEndpoint { Host = registeredType.Transport.Uri, Name = registeredType.Queue }, messageInstance, this.PipelineContext, inboundMessageContext);
            // Record the messageIntent type
            outBoundMessageContext.RawMessage.MetaData.Add(new MetaData { Name = StandardMetaData.MessageIntent, Value = messageIntent.ToString() });

            // Start the inbound message pipeline
            var tasks = this.Configure.Configuration.Pipeline.OutboundMessagePipelineTasks.GetTasks();
            if (tasks.Count > 0)
                await this.ExecuteOutboundMessageTask(outBoundMessageContext, tasks.First);
        }



        async Task ExecuteStartupTask(PipelineContext context, LinkedListNode<PipelineTask> task)
        {

            var taskInstance = this.ServiceLocator.GetInstance(task.Value.Task) as IPipelineStartupTask;
            await taskInstance.InvokeAsync(context, async () =>
            {
                if (task.Next != null)
                    await this.ExecuteStartupTask(context, task.Next);
            });
        }

        async Task ExecuteInboundMessageTask(InboundMessageContext context, LinkedListNode<PipelineTask> task)
        {

            var taskInstance = this.ServiceLocator.GetInstance(task.Value.Task) as IPipelineInboundMessageTask;
            await taskInstance.InvokeAsync(context, async () =>
            {
                
                if (task.Next != null && !context.ShouldTerminatePipeline)
                    await this.ExecuteInboundMessageTask(context, task.Next);
            });
        }
        async Task ExecuteOutboundMessageTask(OutboundMessageContext context, LinkedListNode<PipelineTask> task)
        {

            var taskInstance = this.ServiceLocator.GetInstance(task.Value.Task) as IPipelineOutboundMessageTask;
            await taskInstance.InvokeAsync(context, async () =>
            {
                if (task.Next != null && !context.InboundMessageContext.ShouldTerminatePipeline) 
                    await this.ExecuteOutboundMessageTask(context, task.Next);
            });
        }

        public static string ThisEndpointName {
            get
            {
                return string.Format("{0}-{1}:{2}", Environment.MachineName, Assembly.GetExecutingAssembly().GetName().Name,
                    System.Diagnostics.Process.GetCurrentProcess().Id);
            }
        }
    }
}
