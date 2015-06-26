using System;
using System.Threading.Tasks;
using ABus.Contracts;

namespace ABus.Sample
{
    class AddMetaDataToPaymentMessagesTask : IPipelineInboundMessageTask
    {
        public async Task InvokeAsync(InboundMessageContext context, Func<Task> next)
        {
            // Only apply to messages from the Payments Subscription
            if (context.Queue == "PaymentQueue")
            {
                // Need to add the meta which tells ABus what type to deserialize
                context.RawMessage.MetaData.Add(new MetaData { Name = StandardMetaData.MessageType, Value = "ABus.Sample.Contracts.Payments.MakePaymentCommand" });
                context.RawMessage.MetaData.Add(new MetaData { Name = StandardMetaData.ContentType, Value = "application/json" });
            }

            await next().ConfigureAwait(false);
        }
    }
}
