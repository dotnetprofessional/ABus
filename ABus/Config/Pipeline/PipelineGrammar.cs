namespace ABus.Config.Pipeline
{
    public class PipelineGrammar
    {
        public PipelineGrammar(ConfigurationGrammar parent, string pipelineName)
        {
            this.Parent = parent;
            this.PipelineConfiguration = parent.Configuration.Pipeline;
            this.PipelineName = pipelineName;
        }

        internal ConfigurationGrammar Parent { get; set; }

        internal PipelineConfiguration PipelineConfiguration { get; set; }

        internal string PipelineName { get; set; }
    }
}