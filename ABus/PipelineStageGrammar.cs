using System;

namespace ABus
{
    public class PipelineStageGrammar
    {
        PipelineGrammar Pipeline { get; set; }
        string Name { get; set; }

        public PipelineStageGrammar(PipelineGrammar pipeline, string name)
        {
            this.Pipeline = pipeline;
            this.Name = name;
        }

        public PipelineStageGrammar Register(string taskName, Type task)
        {
            this.Pipeline.AssociatedPipeline.Register(this.Pipeline.PipelineName, this.Name, new PipelineTask(taskName, task));
            return this;
        }

        public Pipeline And()
        {
            return this.Pipeline.AssociatedPipeline;
        }

        public PipelineStageGrammar Then(string taskName, Type task)
        {
            return this.Register(taskName, task);
        } 
    }
}