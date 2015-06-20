using System;
using System.Reflection;
using System.Threading;
using ABus.Contracts;

namespace ABus.Tasks.Outbound
{
    class AppendCommonMetaDataTask : IPipelineOutboundMessageTask
    {
        public void Invoke(OutboundMessageContext context, Action next)
        {
            var raw = context.RawMessage;

            raw.MetaData.Add(new MetaData
            {
                Name = StandardMetaData.SourceEndpoint,
                Value = string.Format("{0}\\{1}:{2}", Environment.MachineName, Assembly.GetExecutingAssembly().GetName().Name,
                        System.Diagnostics.Process.GetCurrentProcess().Id)
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

            next();
        } 
    }
}
