using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ABus.Contracts;

namespace ABus.Tasks.Startup
{
    class InitializeHandlersTask : IPipelineStartupTask
    {
        public void Invoke(PipelineContext context, Action next)
        {
            // Check if transactions have been disabled and output
            if(!context.Configuration.Transactions.TransactionsEnabled)
                context.Trace.Warning("Transaction support disabled!");

            var tasks = new List<Task>();
            foreach (var h in context.RegisteredHandlers)
            {
                var transport = context.TransportInstances[h.MessageType.Transport.Name];
                var endpoint = new QueueEndpoint { Host = h.MessageType.Transport.Uri, Name = h.MessageType.Queue };

                var t = transport.SubscribeAsync(endpoint, h.SubscriptionName);
                tasks.Add(t);
                context.Trace.Verbose(string.Format("Initializing handler: {0} with subscription {1}", h.MessageType.MessageType.Name, h.SubscriptionName));
            }

            // Now wait for all tasks to complete
            try
            {
                Task.WaitAll(tasks.ToArray());
            } 
            catch (AggregateException ex)
            {
                foreach(var e in ex.InnerExceptions)
                    context.Trace.Critical(e.Message);
            }
            
            next();
        }
    }
}
