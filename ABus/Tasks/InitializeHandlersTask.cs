using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABus.Tasks
{
    class InitializeHandlersTask : IPipelineStartupTask
    {
        public void Invoke(PipelineContext context, Action next)
        {
 

            next();
        }
    }
}
