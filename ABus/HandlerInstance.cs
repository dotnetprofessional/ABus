using System;
using System.Reflection;

namespace ABus
{
    public class HandlerInstance
    {
        public string MessageTypeName { get; set; }
        public MethodInfo Method { get; set; }

        /// <summary>
        /// Holds a reference to the method handler instance
        /// </summary>
        /// <remarks>
        /// A weak reference is used to ensure memory leaks are prevented.
        /// </remarks>
        public object TypeInstance { get; set; }

        public Type MessageType { get; set; }
    }
}