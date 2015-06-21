using System;
using ABus.Contracts;

namespace ABus.Tasks.Startup
{
    public class ValidateQueuesTask : IPipelineStartupTask
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
                        // Check if Auditing is enable and if so ensure the auditing queue is available for this transport
                        if (m.Transport.EnableAuditing)
                        {
                            var auditEndpoint = new QueueEndpoint {Host = m.Transport.Uri, Name = m.Transport.AuditQueue};
                            if (!transport.QueueExistsAsync(auditEndpoint).Result)
                            {
                                transport.CreateQueueAsync(auditEndpoint).Wait();

                                // Now need to add a default subscription so that messages are not lost!
                                context.Trace.Information(string.Format("Created: audit queue: {0}:{1}", m.Transport.Name, auditEndpoint.Name));
                            }
                        }
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
 