using System.Collections.ObjectModel;

namespace ABus
{
    public class RegisteredHandlerCollection : KeyedCollection<string, RegisteredHandler>
    {
        protected override string GetKeyForItem(RegisteredHandler item)
        {
            return item.SubscriptionName;
        }
    }
}