using System;
using System.Threading.Tasks;

namespace ABus
{
    public interface IPipelineStartupTask
    {
        Task InvokeAsync(PipelineContext context, Func<Task> next);
    }
} 