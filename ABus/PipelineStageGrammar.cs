namespace ABus
{
    public class PipelineStageGrammar
    {
        Pipeline Pipeline { get; set; }
        string Name { get; set; }

        public PipelineStageGrammar(Pipeline pipeline, string name)
        {
            this.Pipeline = pipeline;
            this.Name = name;
        }

        public PipelineStageGrammar Register<T>()
        {
            this.Pipeline.Register<T>(this.Name);
            return this;
        }

        public Pipeline And()
        {
            return this.Pipeline;
        }

        public PipelineStageGrammar AndAlso<T>()
        {
            this.Pipeline.Register<T>(this.Name);
            return this;
        }
    }
}