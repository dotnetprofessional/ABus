using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ABus.Contracts;

namespace ABus.Tasks.Outbound
{
    class AppendCommonMetaDataTask : IPipelineOutboundMessageTask
    {
        public async Task InvokeAsync(OutboundMessageContext context, Func<Task> next)
        {
            var raw = context.RawMessage;

            raw.MetaData.Add(new MetaData { Name = StandardMetaData.MessageType, Value = context.MessageInstance.GetType().FullName });

            raw.MetaData.Add(new MetaData
            {
                Name = StandardMetaData.SourceEndpoint,
                Value = Pipeline.ThisEndpointName
            });

            // Define the conversation Id for this message
            if (context.InboundMessageContext.Bus.CurrentMessage != null)
            {
                string conversationId = "";
                var inboundMessage = context.InboundMessageContext.Bus.CurrentMessage;
                if (raw.MetaData.Contains(StandardMetaData.ConversationId))
                    // Add existing conversationId
                    inboundMessage.MetaData.Add(raw.MetaData[StandardMetaData.ConversationId]);
                else
                    raw.MetaData.Add(new MetaData
                    {
                        Name = StandardMetaData.ConversationId,
                        Value = Guid.NewGuid().ToString()
                    });

                if(inboundMessage.MessageId != null)
                    raw.MetaData.Add(new MetaData{Name = StandardMetaData.RelatedTo,Value = inboundMessage.MessageId});
            }

            if (Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                raw.MetaData.Add(new MetaData{Name = StandardMetaData.AuthenticatedUser, Value = Thread.CurrentPrincipal.Identity.Name});
                raw.MetaData.Add(new MetaData { Name = StandardMetaData.AuthenticationType, Value = Thread.CurrentPrincipal.Identity.AuthenticationType });
            }
            else
            {
                raw.MetaData.Add(new MetaData { Name = StandardMetaData.AuthenticatedUser, Value = string.Format(@"{0}\{1}", Environment.UserDomainName, Environment.UserName) });
                raw.MetaData.Add(new MetaData { Name = StandardMetaData.AuthenticationType, Value = "Non-Authenticated" });
            }

            // If this message is a send then add a replyTo 
            if (raw.MessageIntent == MessageIntent.Send)
                // This tells the reciever where to send the response back to if a replyTo queue has been defined!
                if (context.PipelineContext.Configuration.ReplyToQueue != null)
                    raw.MetaData.Add(new MetaData {Name = StandardMetaData.ReplyTo, Value = context.PipelineContext.Configuration.ReplyToQueue.Endpoint});

            // Add name of destination queue to message - this is helpful for the audit log to know where the message came from
            var messageType = context.PipelineContext.RegisteredMessageTypes[raw.MetaData[StandardMetaData.MessageType].Value];
            var destinationQueue = messageType.Queue;

            // Reply messages need to have the recieving queue added to the meta-data for the transport to send it to the correct queue
            if (raw.MessageIntent == MessageIntent.Reply)
                // Need to transfer the replyTo queue meta-data to the outgoing reply message
                if (context.InboundMessageContext.RawMessage.MessageIntent == MessageIntent.Send)
                {
                    // Override destination queue as its a reply
                    destinationQueue = context.InboundMessageContext.RawMessage.MetaData[StandardMetaData.ReplyTo].Value;
                    raw.MetaData.Add(new MetaData { Name = StandardMetaData.ReplyTo, Value = destinationQueue});
                }

            // Add name of destination queue to message - this is helpful for the audit log to know where the message came from
            raw.MetaData.Add(new MetaData{Name = StandardMetaData.DestinationEndpoint, Value = destinationQueue});

            await next().ConfigureAwait(false);
        } 
    }
}
