namespace ABus.Config.Pipeline
{
    public class StartupPipelineGrammer : PipelineGrammar
    {
        public PipelineStageGrammar Initialize
        {
            get { return new PipelineStageGrammar(this.Parent, this, StartupStages.Initialize); }
        }

        public StartupPipelineGrammer(ConfigurationGrammar parent, string pipelineName) : base(parent, pipelineName)
        {
        }
    }
}