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
        public int DeliveryCount { get; internal set; }

        public IMessagerBroker Broker { private get; set; }

        public BrokeredMessage(string host, string topic, RawMessage message)
        {
            Host = host;
            Topic = topic;
            Message = Clone(message);
            DeliveryCount = 0;
        }

        public void Abandon()
        {
            // TODO: how to handle Abandon()
            if(Broker != null)
                Broker.Abandon(this);
        }

        public void Complete()
        {
            // TODO: how to handle Complete()
            if (Broker != null)
                Broker.Complete(this);
        }

        public BrokeredMessage Clone()
        {
            var message = new BrokeredMessage(Host, Topic, Message);
            message.Broker = Broker;
            return message;
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
