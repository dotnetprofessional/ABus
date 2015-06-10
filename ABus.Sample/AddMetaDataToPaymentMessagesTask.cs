using System;
using ABus.Contracts;

namespace ABus.Sample
{
    class AddMetaDataToPaymentMessagesTask : IPipelineInboundMessageTask
    {
        public void Invoke(InboundMessageContext context, Action next)
        {
            // Only apply to messages from the Payments Subscription
            if (context.Queue == "PaymentQueue")
            {
                // Need to add the meta which tells ABus what type to deserialize
                context.RawMessage.MetaData.Add(new MetaData { Name = StandardMetaData.MessageType, Value = "ABus.Sample.Contracts.Payments.MakePaymentCommand" });
                context.RawMessage.MetaData.Add(new MetaData { Name = StandardMetaData.ContentType, Value = "application/json" });
            }

            next();
        }
    }
}
