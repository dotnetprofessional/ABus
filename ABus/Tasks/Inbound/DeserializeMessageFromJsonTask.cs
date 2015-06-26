using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ABus.Contracts;
using ABus.Exceptions;
using Newtonsoft.Json;

namespace ABus.Tasks.Inbound
{
    internal class DeserializeMessageFromJsonTask : IPipelineInboundMessageTask
    {
        public async Task InvokeAsync(InboundMessageContext context, Func<Task> next)
        {
            // Check that this message has compatable meta data
            if (context.RawMessage.MetaData.Contains(StandardMetaData.MessageType))
            {
                // Determine the message type and create a type class
                var messageTypeName = context.RawMessage.MetaData[StandardMetaData.MessageType].Value;
                var messageType = context.PipelineContext.RegisteredMessageTypes[messageTypeName].MessageType;

                var json = Encoding.Unicode.GetString(context.RawMessage.Body, 0, context.RawMessage.Body.Length);
                try
                {
                    var msg = JsonConvert.DeserializeObject(json, messageType);
                    context.TypeInstance = msg;
                }
                catch (Exception ex)
                {
                    throw new MessageDeserializationException(string.Format("Error deserializing message from queue {0}", context.Queue), ex);
                }
            }
            else
                throw new MessageDeserializationException(string.Format("Unable to deserialize message from queue {0} as it has no message type defined.", context.Queue));

            await next().ConfigureAwait(false);
        }
    }
}
