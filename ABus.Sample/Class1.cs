using System;
using ABus.Contracts;

namespace ABus.Sample
{
    public class SampleMessageHandler : IHandleMessage<TestMessageCommand>, IHandleMessage<TestMessage2Event>
    {
        public IBus Bus { get; set; }

        public void Handler(TestMessageCommand messageCommand)
        {
            //Console.WriteLine(string.Format("Received message with Id {0} and Name {1}", this.Bus.CurrentMessage.MessageId, message.Name));
            Console.WriteLine(string.Format("Received message with name {0} and address {1}", messageCommand.Name, messageCommand.Addresss));
        }

        public void Handler(string test)
        { 

        } 

        public void Handler(TestMessage2Event message)
        {
            
        }
    }

    public class TestMessageCommand
    {
        public string Name { get; set; }

        public string Addresss { get; set; }
    }

    public class TestMessage2Event
    {
        public string Name { get; set; }

        public string Addresss { get; set; }
    }
}
