using System;
using System.Text;
using System.Threading.Tasks;
using ABus.Contracts;
using Newtonsoft.Json;

namespace ABus.Tasks.Outbound
{
    internal class SerializeMessageToJsonTask : IPipelineOutboundMessageTask
    {
        public async Task InvokeAsync(OutboundMessageContext context, Func<Task> next)
        {
            // Set the content type of the message
            context.RawMessage.MetaData.Add(new MetaData {Name = StandardMetaData.ContentType, Value = "application/json"});

            // Serialize the message to json
            var json = JsonConvert.SerializeObject(context.MessageInstance);

            context.RawMessage.Body = Encoding.Unicode.GetBytes(json);

            await next().ConfigureAwait(false);
        }
    }
}

