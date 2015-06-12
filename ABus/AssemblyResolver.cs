using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ABus.Contracts;
 
namespace ABus
{
    public class AssemblyResolver : IAssemblyResolver
    {
        static List<Assembly> foundAssemblies;

        public List<Assembly> GetAssemblies()
        {
            if (foundAssemblies != null)
                return foundAssemblies;


            var binDirectory = AppDomain.CurrentDomain.BaseDirectory;
            //var binDirectory = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath);

            var knownAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).ToDictionary(k => new Uri(k.CodeBase).LocalPath);

            var assemblyList = from f in Directory.GetFiles(binDirectory)
                where (f.EndsWith(".dll") || f.EndsWith(".exe")) && !(f.Contains("System.") || f.Contains("Microsoft."))
                select f;

            var resolveAssemblies = new List<Assembly>();

            foreach (var a in assemblyList)
            {
                if (!knownAssemblies.ContainsKey(a))
                {
                    // Ensure the assembly is loaded
                    var newAssembly = Assembly.LoadFrom(a);
                    resolveAssemblies.Add(newAssembly);
                }
                else
                    resolveAssemblies.Add(knownAssemblies[a]);
            }

            // Cahce assemblies so we dont have to do it again
            foundAssemblies = resolveAssemblies;

            return resolveAssemblies;
        }
    }
} 
