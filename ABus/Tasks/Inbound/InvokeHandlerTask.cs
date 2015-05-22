using System;

namespace ABus.Tasks.Inbound
{
    class InvokeHandlerTask : IPipelineMessageTask
    {
        public void Invoke(MessageContext context, Action next)
        {
            // Note that each handler gets its own subscription so even if a message type is
            // handled more than once each one of those handlers will have its own subscription
            // this ensures that if one handler fails it doesn't impact the ability for other handlers
            // to process the message.

            // Get the handler for this message based on the subscription
            var handler = context.PipelineContext.RegisteredHandlers[context.SubscriptionName];

            // Need to create a new instance of the class that has the handler
            var typeInstance = context.PipelineContext.ServiceLocator.GetInstance(handler.ClassType);
            handler.Method.Invoke(typeInstance, new[] { context.TypeInstance });

            context.PipelineContext.Trace.Verbose(string.Format("Invoked handler: {0}.{1} ", handler.ClassType.Name, context.TypeInstance.GetType().Name));

            next();
        }
    }
}
