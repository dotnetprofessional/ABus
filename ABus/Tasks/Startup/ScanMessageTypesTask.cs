using System;
using System.Linq;
using ABus.Contracts;

namespace ABus.Tasks.Startup
{
    public class ScanMessageTypesTask : IPipelineStartupTask
    {
        public IAssemblyResolver AssemblyResolver { get; set; }

        public ScanMessageTypesTask(IAssemblyResolver assemblyResolver)
        {
            AssemblyResolver = assemblyResolver;
        }

        public void Invoke(PipelineContext context, Action next)
        {
            var assemblies = this.AssemblyResolver.GetAssemblies();
            var messageTypes = (from a in assemblies
                from t in a.GetTypes()
                where t.IsClass && (t.Name.EndsWith("Command") || t.Name.EndsWith("Event"))
                // Find types that are either commands or events
                select t).Distinct();

            foreach (var messageType in messageTypes)
            {
                var registeredMessageType = new RegisteredMessageType();
                registeredMessageType.FullName = messageType.FullName;
                registeredMessageType.MessageType = messageType;

                context.RegisteredMessageTypes.Add(registeredMessageType);

                context.Trace.Verbose(string.Format("Message {0} type found.", registeredMessageType.FullName));
            }
            next(); 
        } 
    } 
}
