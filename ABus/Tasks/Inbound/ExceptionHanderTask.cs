using System;

namespace ABus.Tasks.Inbound
{
    class ExceptionHanderTask : IPipelineInboundMessageTask
    {
        public ExceptionHanderTask()
        {
            this.RetryCount = 0;
        }

        public int RetryCount { get; set; }

        public void Invoke(InboundMessageContext context, Action next)
        {
            try
            {
                next();
            }
            catch (Exception)
            {
                if (this.RetryCount <= 5)
                    this.Invoke(context, next);
                else
                {
                    this.RetryCount ++;
                    throw;
                } 
            }
        }
    }
}
