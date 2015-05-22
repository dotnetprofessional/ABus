using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace ABus
{
    public class RegisteredHandlerCollection : KeyedCollection<string, RegisteredHandler>
    {
        protected override string GetKeyForItem(RegisteredHandler item)
        {
            return item.SubscriptionName;
        }
    }

    public class RegisteredHandler
    {
        /// <summary>
        /// The message type to be handled
        /// </summary>
        public RegisteredMessageType MessageType { get; set; } 

        /// <summary>
        /// The class that implements the handler
        /// </summary>
        public Type ClassType { get; set; }

        /// <summary>
        /// The method info of the handler for this message type
        /// </summary>
        public MethodInfo Method { get; set; }

        /// <summary>
        /// The name of the subsciption for this handler
        /// </summary>
        /// <remarks>
        /// This subscription is associated with the queue defined by <see cref="MessageType"/>
        /// </remarks>
        public string SubscriptionName { get { return string.Format("{0}.{1}", this.ClassType.Name, this.MessageType.MessageType.Name); } }
    }
} 