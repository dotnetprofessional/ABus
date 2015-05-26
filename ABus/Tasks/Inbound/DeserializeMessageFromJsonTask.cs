using System;
using System.Text;
using ABus.Contracts;
using Newtonsoft.Json;

namespace ABus.Tasks.Inbound
{
    class DeserializeMessageFromJsonTask : IPipelineInboundMessageTask
    {
        public void Invoke(InboundMessageContext context, Action next)
        {
            // Determine the message type and create a type class
            var messageTypeName = context.RawMessage.MetaData[StandardMetaData.MessageType].Value;
            var messageType = context.PipelineContext.RegisteredMessageTypes[messageTypeName].MessageType;

            var json = Encoding.Unicode.GetString(context.RawMessage.Body, 0, context.RawMessage.Body.Length);
            var msg = JsonConvert.DeserializeObject(json, messageType);

            context.TypeInstance = msg;

            next();
        }
    }
}
