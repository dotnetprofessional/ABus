using System;

namespace ABus.Exceptions
{
    public class MessageDeserializationException : Exception
    {
        public MessageDeserializationException(string message) : base(message)
        {
        }

        public MessageDeserializationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
