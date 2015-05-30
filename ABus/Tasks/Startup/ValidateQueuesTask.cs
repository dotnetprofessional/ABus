using System;
using ABus.Contracts;

namespace ABus.Tasks.Startup
{
    class ValidateQueuesTask : IPipelineStartupTask
    {
        public void Invoke(PipelineContext context, Action next)
        {
            foreach (var m in context.RegisteredMessageTypes)
            {
                // get message transport
                var transport = context.TransportInstances[m.Transport.Name];
                var endpoint = new QueueEndpoint {Host = m.Transport.Uri, Name = m.Queue};
                var exists = transport.QueueExistsAsync(endpoint).Result;
                if (exists)
                    context.Trace.Verbose(string.Format("Found: queue: {0}:{1}", m.Transport.Name, m.Queue));
                else
                {
                    context.Trace.Critical(string.Format("NOT FOUND: queue: {0}:{1}", m.Transport.Name, m.Queue));
                    if (context.Configuration.EnsureQueuesExist)
                    {
                        // Create queue
                        transport.CreateQueueAsync(endpoint).Wait();
                        context.Trace.Information(string.Format("Created: queue: {0}:{1}", m.Transport.Name, m.Queue));
                    }
                }
            }
             
            next(); 
        }
    } 
}
 