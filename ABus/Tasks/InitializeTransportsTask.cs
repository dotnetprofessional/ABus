using System;
using ABus.Contracts;

namespace ABus.Tasks
{
    class InitializeTransportsTask : IPipelineStartupTask
    {
        public void Invoke(PipelineContext context, Action next)
        {
            foreach (var t in context.AvailableTransports)
            {
                var type = Type.GetType(t.Transport.TransportType);
                var transport = context.ServiceLocator.GetInstance(type) as IMessageTransport;
                transport.ConfigureHost(t);
                context.TransportInstances.Add(t.Name, transport);
            }

            next();
        }
    }
} 
