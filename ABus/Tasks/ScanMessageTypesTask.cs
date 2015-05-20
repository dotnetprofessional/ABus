using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using ABus.Contracts;

namespace ABus.Tasks
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
                registeredMessageType.Name = messageType.Name;
                registeredMessageType.Path = messageType.FullName;

                context.RegisteredMessageTypes.Add(registeredMessageType);

                Trace.WriteLine(string.Format("Message {0} type found.", registeredMessageType.Name));
            }
            next();
        }
    } 
}
