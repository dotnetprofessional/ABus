using System;

namespace ABus.Contracts
{
    public class RawMessageReceivedEventArgs : EventArgs
    {
        public RawMessage Message { get; set; }

        public RawMessageReceivedEventArgs(RawMessage message)
        {
            this.Message = message;
        }
    }
}