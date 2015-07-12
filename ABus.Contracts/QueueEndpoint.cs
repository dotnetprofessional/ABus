namespace ABus.Contracts
{
    public class QueueEndpoint
    {
        public QueueEndpoint()
        {
            this.SubQueueName = "";
        }

        public string Host { get; set; }

        public string Name { get; set; }

        public string SubQueueName { get; set; }
        public override string ToString()
        {
            return string.Format("{0}:{1}", this.Host, this.Name);
        }

        public void SetQueueName(string queueName)
        {
            if (queueName.Contains(@"\"))
            {
                var parts = queueName.Split('\\');
                this.Name = parts[0];
                this.SubQueueName = parts[1];
            }
            else
                this.Name = queueName;
        }
    }
}
