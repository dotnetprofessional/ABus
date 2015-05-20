using System;

namespace ABus.Tasks
{
    class AssignTransportToMessageTypesTask : IPipelineStartupTask
    {
        public void Invoke(PipelineContext context, Action next)
        {
            // TODO: Add better logic that allows different message types to be associated with different transports via configuration
            foreach (var m in context.RegisteredMessageTypes)
            {
                // Strip off the suffix of Command and Event from queue names
                m.Queue = m.Path.Replace("Command","").Replace("Event","");
                m.Transport = context.AvailableTransports[0];
            }

            next();
        }
    }
} 
