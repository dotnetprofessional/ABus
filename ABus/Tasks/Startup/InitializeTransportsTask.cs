using System;
using System.Threading.Tasks;
using ABus.Contracts;

namespace ABus.Tasks.Startup
{
    class InitializeTransportsTask : IPipelineStartupTask
    {
        public async Task InvokeAsync(PipelineContext context, Func<Task> next)
        {
            foreach (var t in context.Configuration.AvailableTransports)
            {
                var type = Type.GetType(t.TransportType);
                var transport = context.ServiceLocator.GetInstance(type) as IMessageTransport;
                transport.ConfigureHost(t);
                context.TransportInstances.Add(t.Name, transport);

                transport.MessageReceived += context.RaiseMessageReceivedEvent;
            }
              
            await next().ConfigureAwait(false);
        } 
    }
} 
