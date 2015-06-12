using System.Collections.ObjectModel;
using ABus.Contracts;

namespace ABus
{
    public class RegisteredMessageTypeCollection : KeyedCollection<string, RegisteredMessageType>
    {
        protected override string GetKeyForItem(RegisteredMessageType item)
        {
            return item.FullName;
        }
    }
}