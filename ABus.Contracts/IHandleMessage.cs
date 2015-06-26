using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABus.Contracts
{
    public interface IHandleMessage<T>
    {
        Task HandlerAsync(T message);
    }
}
