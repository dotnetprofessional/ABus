using System;
using System.Text;
using ABus.Contracts;
using Newtonsoft.Json;

namespace ABus.Tasks.Outbound
{
    internal class SerializeMessageToJsonTask : IPipelineOutboundMessageTask
    {
        public void Invoke(OutboundMessageContext context, Action next)
        {
            // Set the content type of the message
            context.RawMessage.MetaData.Add(new MetaData {Name = StandardMetaData.MessageType, Value = context.MessageInstance.GetType().FullName});
            context.RawMessage.MetaData.Add(new MetaData {Name = StandardMetaData.ContentType, Value = "application/json"});

            // Serialize the message to json
            var json = JsonConvert.SerializeObject(context.MessageInstance);

            context.RawMessage.Body = Encoding.Unicode.GetBytes(json);

            next();
        }
    }
}

