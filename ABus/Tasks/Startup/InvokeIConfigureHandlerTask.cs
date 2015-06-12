using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using ABus.Contracts;

namespace ABus.Tasks.Startup
{
    class InvokeIConfigureHandlerTask : IPipelineStartupTask
    {
        public void Invoke(PipelineContext context, Action next)
        {
            var interfaceType = typeof(IConfigureHandler<>);
            var handlers = GetTypesImplementingInterface(interfaceType);

            foreach (var handler in handlers)
            {
                var handlerInterfaces = handler.GetTypeInfo().ImplementedInterfaces;

                // Only deal with implementations of IHandleMessage interfaces
                var interfaces = handlerInterfaces.Where(i => i.Name == "IConfigureHandler`1");
                foreach (var interfaceImplementation in interfaces)
                {

                    // Get the message type used
                    var argumentType = interfaceImplementation.GenericTypeArguments[0];
                    var method = interfaceImplementation.GetTypeInfo().DeclaredMethods.First();

                    context.Trace.Verbose(string.Format("Configuring Handler {0} for type {1}", handler.FullName, argumentType.Name));
                    // Need to create a new instance of the class that has the handler
                    var typeObject = Activator.CreateInstance(handler);

                    var handlerKey = string.Format("{0}.{1}", handler.Name, argumentType.Name);
                    var parameterData = context.RegisteredHandlers[handlerKey];


                    method.Invoke(typeObject, new object[] { parameterData });
                }
            }

            next();
        }
        static IEnumerable<Type> GetTypesImplementingInterface(Type type)
        {
            // TODO: Had to hardcode Assembly resolver here to avoid having to specify the IoC container
            //       Need to think about how the AssemblyResolver should be leveraged.

            //var assemblyResolver = this.ServiceLocator.GetInstance<IAssemblyResolver>();
            var assemblyResolver = new AssemblyResolver();
            var assemblies = assemblyResolver.GetAssemblies();

            var typesFound = (from a in assemblies
                            from t in a.GetTypes()                              // Get a list of all types within each assembly
                            from i in t.GetTypeInfo().ImplementedInterfaces     // Check TypeInfo for type 
                            where i.Name == "IConfigureHandler`1"      //and only select those that implement IHandler(T message)
                            select t).Distinct();

            return typesFound;
        }
    }
}
