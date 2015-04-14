using System;

namespace ABus.Contracts
{
    public class TransportException : Exception
    {
        public TransportException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}