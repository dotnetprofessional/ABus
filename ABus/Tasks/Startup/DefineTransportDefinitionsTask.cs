using System;

namespace ABus.Tasks.Startup
{
    internal class DefineTransportDefinitionsTask : IPipelineStartupTask
    {
        public void Invoke(PipelineContext context, Action next)
        {
            // TODO: Add code that can read a config to add Transport definitions that not specified in code.
            
            next();
        }
    }
}
