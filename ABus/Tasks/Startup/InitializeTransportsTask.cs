using System;
using ABus.Contracts;

namespace ABus.Tasks.Startup
{
    class InitializeTransportsTask : IPipelineStartupTask
    {
        public void Invoke(PipelineContext context, Action next)
        {
            foreach (var t in context.Configuration.AvailableTransports)
            {
                var type = Type.GetType(t.TransportType);
                var transport = context.ServiceLocator.GetInstance(type) as IMessageTransport;
                transport.ConfigureHost(t);
                context.TransportInstances.Add(t.Name, transport);

                transport.MessageReceived += context.RaiseMessageReceivedEvent;
            }
              
            next();
        } 
    }
} 
