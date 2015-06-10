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
            try
            {
                next();
            }
            catch (MessageDeserializationException ex)
            {
                // Unable to recover from this message so record the exception
                context.RawMessage.MetaData.Add(new MetaData{Name = StandardMetaData.ContentType, Value = JsonConvert.SerializeObject(ex)});

                context.PipelineContext.Trace.Error("MessageDeserializationException: " + ex.Message);
                context.PipelineContext.Trace.Warning("Message has been consumed as error queue has yet to be implemented!");
                // TODO: Need to transfer this message to the error queue in a transactionally safe way
            }
            catch (Exception)
            {
                if (this.RetryCount <= 5)
                {
                    this.RetryCount ++;
                    this.Invoke(context, next);
                } 
                else
                    throw;
            }
        }
    }
}
