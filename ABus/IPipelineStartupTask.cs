using System;
using System.Threading.Tasks;

namespace ABus
{
    public interface IPipelineStartupTask
    {
        void Invoke(PipelineContext context, Action next);
    }
} 