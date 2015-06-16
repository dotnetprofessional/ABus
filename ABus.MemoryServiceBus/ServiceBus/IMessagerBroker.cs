using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ABus.MemoryServiceBus.ServiceBus
{
    public interface IMessagerBroker
    {
        void Acquire(BrokeredMessage message);
        void Abandon(BrokeredMessage message);
        void Complete(BrokeredMessage message);
    }
}
