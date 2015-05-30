namespace ABus.Config.Pipeline
{
    public class OutboundMessagePipelineGrammer : PipelineGrammar
    {
        public PipelineStageGrammar ValidateBestPractices
        {
            get { return new PipelineStageGrammar(this.Parent, this, OutboundMessageStages.ValidateBestPractices); }
        }
        
        public PipelineStageGrammar TransformInboundRawMessage
        {
            get { return new PipelineStageGrammar(this.Parent, this, OutboundMessageStages.TransformOutboundRawMessage); }
        }

        public PipelineStageGrammar CreateRawMessage
        {
            get { return new PipelineStageGrammar(this.Parent, this, OutboundMessageStages.CreateRawMessage); }
        }

        public PipelineStageGrammar Serialize
        {
            get { return new PipelineStageGrammar(this.Parent, this, OutboundMessageStages.Serialize); }
        }

        public PipelineStageGrammar TransformInboundMessage
        {
            get { return new PipelineStageGrammar(this.Parent, this, OutboundMessageStages.TransformOutboundMessage); }
        }


        public PipelineStageGrammar SendMessage
        {
            get { return new PipelineStageGrammar(this.Parent, this, OutboundMessageStages.SendMessage); }
        }

        public OutboundMessagePipelineGrammer(ConfigurationGrammar parent, string pipelineName)
            : base(parent, pipelineName)
        {
        }
    }
}