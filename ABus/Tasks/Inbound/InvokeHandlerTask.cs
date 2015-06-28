using System;
using System.Linq;
using System.Threading.Tasks;
using ABus.Contracts;

namespace ABus.Tasks.Inbound
{
    class InvokeHandlerTask : IPipelineInboundMessageTask
    {
        public async Task InvokeAsync(InboundMessageContext context, Func<Task> next)
        {
            // Note that each handler gets its own subscription so even if a message type is
            // handled more than once each one of those handlers will have its own subscription
            // this ensures that if one handler fails it doesn't impact the ability for other handlers
            // to process the message.

            // Get the handler for this message based on the subscription
            var messageTypeFulllname = context.RawMessage.MetaData[StandardMetaData.MessageType].Value;

            // Search for the correct handler based on the message type and the queue
            // This is necessary as a queue may support many message types.
            var handler = context.PipelineContext.RegisteredHandlers.SingleOrDefault(h => h.SubscriptionName ==
                                                                                          context.SubscriptionName &&
                                                                                          h.MessageType.MessageType.FullName == messageTypeFulllname);

            // Need to create a new instance of the class that has the handler
            var typeInstance = context.PipelineContext.ServiceLocator.GetInstance(handler.ClassType);

            // Set the Bus property if one has been defined
            this.SetBusIfRequested(context, typeInstance);

            var task = handler.Method.Invoke(typeInstance, new[] { context.TypeInstance }) as Task;
            await task;

            context.PipelineContext.Trace.Verbose(string.Format("Invoked handler: {0}.{1} ", handler.ClassType.Name, context.TypeInstance.GetType().Name));

            await next().ConfigureAwait(false);
        }

        void SetBusIfRequested(InboundMessageContext context, object typeInstance)
        {
            // Locate first property that implements IBus
            var propertyQuery = from p in typeInstance.GetType().GetProperties()
                where p.PropertyType == typeof (IBus)
                select p;

            var property = propertyQuery.FirstOrDefault();
            if (property != null)
            {
                property.SetValue(typeInstance, context.Bus);
            }
        }
    }
}
