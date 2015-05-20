using System;
using System.Diagnostics;
using ABus.Contracts;

namespace ABus.Tasks
{
    class ValidateQueuesTask : IPipelineStartupTask
    {
        public void Invoke(PipelineContext context, Action next)
        {
            foreach (var m in context.RegisteredMessageTypes)
            {
                // get message transport
                var transport = context.TransportInstances[m.Transport.Name];
                var exists = transport.QueueExists(new QueueEndpoint {Host = m.Transport.Uri, Name = m.Queue}).Result;
                if (exists)
                    Trace.TraceWarning(string.Format("Found: queue: {0}:{1}", m.Transport.Name, m.Queue));
                else
                    Trace.TraceError(string.Format("NOT FOUND: queue: {0}:{1}", m.Transport.Name, m.Queue));
            }

            next();
        }
    } 
}
