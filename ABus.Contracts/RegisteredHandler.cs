using System;
using System.Reflection;

namespace ABus.Contracts
{
    public class RegisteredHandler
    {
        public RegisteredHandler(string handlerKey)
        {
            this.HandlerKey = handlerKey;
        }

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
        public string SubscriptionName { get;set; }

        /// <summary>
        /// This value uniquely identifies the handler for this message
        /// </summary>
        public string HandlerKey { get; private set; }
    }
} 