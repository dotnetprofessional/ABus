using ABus.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABus.MemoryServiceBus.ServiceBus
{
    public class BrokeredMessage
    {
        public string Host { get; private set; }
        public string Topic { get; private set; }
        public RawMessage Message { get; private set; }

        public BrokeredMessage(string host, string topic, RawMessage message)
        {
            Host = host;
            Topic = topic;
            Message = Clone(message);
        }

        RawMessage Clone(RawMessage message)
        {
            return new RawMessage()
            {
                Body = (byte[])message.Body.Clone(),
                MessageId = message.MessageId,
                MetaData = Clone(message.MetaData),
                TimeToBeReceived = message.TimeToBeReceived
            };
        }

        MetaDataCollection Clone(MetaDataCollection metadata)
        {
            var collection = new MetaDataCollection();
            foreach (var item in metadata)
                collection.Add(item);
            return collection;
        }
    }
}
