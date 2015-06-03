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
        public List<Assembly> GetAssemblies()
        {
            var binDirectory = Directory.GetCurrentDirectory();
            //var binDirectory = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath);

            var assemblyList = from f in Directory.GetFiles(binDirectory)
                               where (f.EndsWith(".dll") | f.EndsWith(".exe")) && !(f.Contains("Microsoft.") || f.Contains("System."))
                               select f;

            var resolveAssemblies = new List<Assembly>();

            foreach (var a in assemblyList)
            {
                // Ensure the assembly is loaded
                var newAssembly = Assembly.LoadFrom(a);
                resolveAssemblies.Add(newAssembly);
            }
            return resolveAssemblies;
        }
    }
} 
