using System;
using System.Linq;
using System.Runtime.InteropServices;
using ABus.Contracts;

namespace ABus.Tasks.Startup
{
    class AssignTransportToMessageTypesTask : IPipelineStartupTask
    {
        public void Invoke(PipelineContext context, Action next)
        {
            // Rules
            // Message endpoint configuration is considered in the order in which it is defined
            // The first match will be taken, when no matches are found then the default will be used.

            var config = context.Configuration;
            if (config.AvailableTransports.Count == 0)
                throw new ArgumentException("At least one transport must be specified in configuration.");

            foreach (var m in context.RegisteredMessageTypes)
            {
                var defaultPattern = config.MessageEndpointDefinitions.FirstOrDefault(d => d.TypePattern == "default");
                foreach (var e in context.Configuration.MessageEndpointDefinitions)
                {
                    var fullTypeName = m.MessageType.FullName;
                    if (fullTypeName.StartsWith(e.TypePattern))
                    {
                        m.Transport = config.AvailableTransports[e.TransportName];
                        m.Queue = e.Endpoint;
                        break;
                    }

                }

                // If no pattern was found apply one of a few options for defaults
                if (m.Transport == null)
                    if (defaultPattern != null)
                    {
                        var transport = config.AvailableTransports[defaultPattern.TransportName];
                        m.Transport = transport;
                    }
                    else if (config.AvailableTransports.Count > 1)
                        throw new ArgumentException("A default message endpoint definition must be specified when defining more than one transport.");
                    else
                        m.Transport = config.AvailableTransports[0];

                // If no endpoint was found apply one of a few options for defaults
                if (m.Queue == null)
                    if (defaultPattern != null && !string.IsNullOrEmpty(defaultPattern.Endpoint))
                        m.Queue = defaultPattern.Endpoint;
                    else
                    // Strip off the suffix of Command and Event from queue names
                        m.Queue = m.MessageType.FullName.Replace("Command", "").Replace("Event", "");
            }

            next();
        }
    }
} 
