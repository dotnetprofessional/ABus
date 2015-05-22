namespace ABus
{
    public class PipelineConfiguration
    {
        public PipelineConfiguration()
        {
            this.EnsureQueuesExist = true;
        }

        public bool EnsureQueuesExist { get;set; }
    }
}  