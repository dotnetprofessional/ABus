using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ABus.InMemoryServiceBus
{
    public interface IBusQueue
    {
        void Send();
    }
}
