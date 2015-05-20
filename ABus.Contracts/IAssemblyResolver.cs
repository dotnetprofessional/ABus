using System.Collections.Generic;
using System.Reflection;

namespace ABus.Contracts
{
    public interface IAssemblyResolver
    {
        List<Assembly> GetAssemblies();
    }
} 
