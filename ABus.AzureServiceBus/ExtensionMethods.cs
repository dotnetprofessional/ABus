using System;
using ABus.Contracts;

namespace ABus.AzureServiceBus
{
    internal static class ExtensionMethods
    {
        public static string SubscriptionKey(this QueueEndpoint endPoint, string name)
        {
            return string.Format("{0}::{1}", endPoint.Name, name);
        }
    }
}
