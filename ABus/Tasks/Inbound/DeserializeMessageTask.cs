using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABus.Contracts;
using Newtonsoft.Json;

namespace ABus.Tasks.Inbound
{
    class DeserializeMessageTask : IPipelineMessageTask
    {
        public void Invoke(MessageContext context, Action next)
        {
            // Determine the message type and create a type class
            var messageTypeName = context.RawMessage.MetaData[StandardMetaData.MessageType].Value;
            var messageType = context.PipelineContext.RegisteredMessageTypes[messageTypeName].MessageType;

            var json = System.Text.Encoding.Unicode.GetString(context.RawMessage.Body, 0, context.RawMessage.Body.Length);
            var msg = JsonConvert.DeserializeObject(json, messageType);

            context.TypeInstance = msg;

            next();
        }
    }
}
