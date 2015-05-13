using System;

namespace ABus
{
    public interface IPipelineStartupTask
    {
        void Invoke(PipelineContext context, Action next);
    }
}