using System;
using ABus.Contracts;
using ABus.Exceptions;
using Newtonsoft.Json;

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
            var deadLetterMessage = false;
            Exception exceptionToAppend = null;

            try
            {
                next();
            }
            catch (MessageDeserializationException ex)
            {
                // Unable to recover from this message so record the exception
                exceptionToAppend = ex;

                context.PipelineContext.Trace.Error("MessageDeserializationException: " + ex.Message);
                deadLetterMessage = true;
            }
            catch (Exception ex)
            {
                // Retry 3 times then dead-letter
                if (this.RetryCount <= 3)
                {
                    context.PipelineContext.Trace.Error(string.Format("Error occured retry {0} attempt: ", this.RetryCount + 1));
                    this.RetryCount++;
                    this.Invoke(context, next);
                }
                else
                {
                    exceptionToAppend = ex;
                    deadLetterMessage = true;
                }
            }

            if (exceptionToAppend != null)
                context.RawMessage.MetaData.Add(new MetaData { Name = StandardMetaData.Exception, Value = JsonConvert.SerializeObject(exceptionToAppend) });

            
            if (deadLetterMessage)
            {
                context.PipelineContext.Trace.Error(string.Format("Transferring message from subscription {0} with Id {1} to error queue",context.SubscriptionName, context.RawMessage.MessageId));
                context.Bus.DeadLetterMessage();
            }
        }
    }
}
