using System.Collections.Generic;
using ABus.Config;
using ABus.Contracts;
using Microsoft.Practices.ServiceLocation;

namespace ABus
{
    public class Pipeline
    {
        IServiceLocator ServiceLocator { get; set; }

        ABusTraceSource Trace { get; set; }
        PipelineContext PipelineContext { get; set; }

        //BlockingCollection<IPipelineTask> Tasks;

        public Pipeline(IServiceLocator serviceLocator)
        {
            this.ServiceLocator = serviceLocator;

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
            this.PipelineContext = new PipelineContext(this.ServiceLocator, this.Configure.Configuration, this.Trace);
            this.PipelineContext.MessageReceivedHandler += InboundMessageReceived;

            var tasks = this.Configure.Configuration.Pipeline.StartupPipelineTasks.GetTasks();
            if(tasks.Count > 0)
                this.ExecuteStartupTask(this.PipelineContext, tasks.First);
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
                if (task.Next != null)
                    this.ExecuteInboundMessageTask(context, task.Next);
            });
        }
        void ExecuteOutboundMessageTask(OutboundMessageContext context, LinkedListNode<PipelineTask> task)
        {

            var taskInstance = this.ServiceLocator.GetInstance(task.Value.Task) as IPipelineOutboundMessageTask;
            taskInstance.Invoke(context, () =>
            {
                if (task.Next != null) 
                    this.ExecuteOutboundMessageTask(context, task.Next);
            });
        }
    }
}
