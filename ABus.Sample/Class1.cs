using System;
using ABus.Contracts;

namespace ABus.Sample
{
    public class SampleMessageHandler : IHandleMessage<TestMessage>, IHandleMessage<TestMessage2>
    {
        public IBus Bus { get; set; }

        public void Handler(TestMessage message)
        {
            //Console.WriteLine(string.Format("Received message with Id {0} and Name {1}", this.Bus.CurrentMessage.MessageId, message.Name));
            Console.WriteLine(string.Format("Received message with Id {0} and Name {1}", 1, message.Name));
        }

        public void Handler(string test)
        {

        }

        public void Handler(TestMessage2 message)
        {
            
        }
    }

    public class TestMessage
    {
        public string Name { get; set; }

        public string Addresss { get; set; }
    }

    public class TestMessage2
    {
        public string Name { get; set; }

        public string Addresss { get; set; }
    }
}
