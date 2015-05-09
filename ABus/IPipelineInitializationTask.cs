using System;

namespace ABus
{
    public interface IPipelineInitializationTask
    {
        void Invoke(PipelineContext context, Action next);
    }
}