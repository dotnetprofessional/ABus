namespace ABus.Config.Pipeline
{
    public class PipelineStageGrammar
    {
        ConfigurationGrammar Parent { get; set; }
        PipelineGrammar PipelineGrammar { get; set; }
        string Name { get; set; }

        public PipelineStageGrammar(ConfigurationGrammar parent, PipelineGrammar pipelineGrammar, string name)
        {
            this.Parent = parent;
            this.PipelineGrammar = pipelineGrammar;
            this.Name = name;
        }

        public PipelineStageGrammar Register<T>()
        {
            var task = typeof (T);
            this.PipelineGrammar.PipelineConfiguration.Register(this.PipelineGrammar.PipelineName, this.Name, new PipelineTask(task.Name, task));
            return this;
        }

        public PipelineConfigurationGrammar AndAlso()
        {
            return this.Parent.Pipeline;
        }

        public ConfigurationGrammar And()
        {
            return this.Parent;
        }

        public PipelineStageGrammar Then<T>()
        {
            return this.Register<T>();
        } 
    }
}