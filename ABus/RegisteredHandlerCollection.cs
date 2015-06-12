using System.Collections.ObjectModel;
using ABus.Contracts;

namespace ABus
{
    public class RegisteredHandlerCollection : KeyedCollection<string, RegisteredHandler>
    {
        protected override string GetKeyForItem(RegisteredHandler item)
        {
            return item.HandlerKey;
        }
    }
}