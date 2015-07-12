using System;
using System.Threading.Tasks;
using ABus.Contracts;
using ABusSample.Contracts;

namespace ABus.Sample.Client
{
    class Handlers : IHandleReplyMessage<TestMessageCommandResponse>
    {
        public IBus Bus { get; set; }

        public Handlers(IBus bus)
        {
            this.Bus = bus;
        }

        public async Task HandlerAsync(TestMessageCommandResponse message)
        {
            Console.WriteLine("Received Reply for message: {0} with the message {1}", this.Bus.CurrentMessage.CorrelationId, message.Message);
        }
    }
}
