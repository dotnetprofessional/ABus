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
        public PipelineStageGrammar RegisterBefore<TExisting, T>()
        {
            var task = typeof(T);
            var exisintTask = typeof (TExisting);
            this.PipelineGrammar.PipelineConfiguration.RegisterBefore(this.PipelineGrammar.PipelineName, this.Name, new PipelineTask(exisintTask.Name, exisintTask), new PipelineTask(task.Name, task));
            return this;
        }

        public PipelineStageGrammar RegisterAfter<TExisting, T>()
        {
            var task = typeof(T);
            var exisintTask = typeof(TExisting);
            this.PipelineGrammar.PipelineConfiguration.RegisterAfter(this.PipelineGrammar.PipelineName, this.Name, new PipelineTask(exisintTask.Name, exisintTask), new PipelineTask(task.Name, task));
            return this;
        }
        public PipelineStageGrammar RegisterReplace<TExisting, T>()
        {
            var task = typeof(T);
            var exisintTask = typeof(TExisting);
            this.PipelineGrammar.PipelineConfiguration.RegisterReplace(this.PipelineGrammar.PipelineName, this.Name, new PipelineTask(exisintTask.Name, exisintTask), new PipelineTask(task.Name, task));
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

        public PipelineStageGrammar ThenBefore<TExisting, T>()
        {
            return this.RegisterBefore<TExisting, T>();
        }
        public PipelineStageGrammar ThenAfter<TExisting, T>()
        {
            return this.RegisterAfter<TExisting, T>();
        }
        public PipelineStageGrammar ThenReplace<TExisting, T>()
        {
            return this.RegisterReplace<TExisting, T>();
        }
    }
}

