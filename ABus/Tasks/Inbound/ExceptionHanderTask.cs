using System;
using System.Threading.Tasks;
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

        public async Task InvokeAsync(InboundMessageContext context, Func<Task> next)
        {
            var deadLetterMessage = false;
            Exception exceptionToAppend = null;

            try
            {
                await next().ConfigureAwait(false);
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
                    // Need to reset the InBoundPipeline before retrying
                    context.Reset();

                    context.PipelineContext.Trace.Error(string.Format("Error occured retry {0} attempt: ", this.RetryCount + 1));
                    this.RetryCount++;

                    // Can't await here - update for C#6
                    this.RetryTask(context, next).Wait();
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

        async Task RetryTask(InboundMessageContext context, Func<Task> next)
        {
            await this.InvokeAsync(context, next).ConfigureAwait(false);
        }
    }
}
