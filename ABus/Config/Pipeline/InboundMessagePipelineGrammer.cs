namespace ABus.Config.Pipeline
{
    public class InboundMessagePipelineGrammer : PipelineGrammar
    {
        
        public PipelineStageGrammar Security
        {
            get { return new PipelineStageGrammar(this.Parent, this, InboundMessageStages.Security); }
        }

        public PipelineStageGrammar TransactionManagement
        {
            get { return new PipelineStageGrammar(this.Parent, this, InboundMessageStages.TransactionManagement); }
        }

        public PipelineStageGrammar TransformInboundRawMessage
        {
            get { return new PipelineStageGrammar(this.Parent, this, InboundMessageStages.TransformInboundRawMessage); }
        }

        public PipelineStageGrammar Deserialize
        {
            get { return new PipelineStageGrammar(this.Parent, this, InboundMessageStages.Deserialize); }
        }

        public PipelineStageGrammar TransformInboundMessage
        {
            get { return new PipelineStageGrammar(this.Parent, this, InboundMessageStages.TransformInboundMessage); }
        }

        public PipelineStageGrammar ExecuteHandler
        {
            get { return new PipelineStageGrammar(this.Parent, this, InboundMessageStages.ExecuteHandler); }
        }

        public InboundMessagePipelineGrammer(ConfigurationGrammar parent, string pipelineName)
            : base(parent, pipelineName)
        {
        }
    }
}