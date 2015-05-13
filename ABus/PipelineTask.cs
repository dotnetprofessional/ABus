using System;

namespace ABus
{
    public class PipelineTask
    {
        public PipelineTask(string name, Type task)
        {
            Name = name;
            Task = task;
        }

        public string Name { get; private set; }

        public Type Task { get; private set; }

    }
}