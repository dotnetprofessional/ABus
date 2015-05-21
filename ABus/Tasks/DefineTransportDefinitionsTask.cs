using System;
using System.Diagnostics;
using ABus.Contracts;

namespace ABus.Tasks
{
    class DefineTransportDefinitionsTask : IPipelineStartupTask
    {
        public void Invoke(PipelineContext context, Action next)
        {
            // TODO: Obtain this from configuration
            var host = new TransportDefinition
            {
                Name = "Demo.BC",
                Uri = "sb://abus-dev.servicebus.windows.net",
                Credentials = "SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=uyauQw6sme25rx0EzLc/2VSWafIF6PROzdkZ9A4N918=",
                Transport = new Transport { Name = "AzureServiceBus", TransportType = "ABus.AzureServiceBus.AzureBusTransport, ABus.AzureServiceBus" }
            };

            context.AvailableTransports.Add(host);
            context.Trace.Verbose(string.Format("Transport definition: {0} using transport {1}", host.Name, host.Transport.Name));
            
            next();
        }
    } 
} 
