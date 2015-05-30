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

            this.Bus.Send(new TestMessage2Event{Name = "Test Send of a message!", Addresss = "No Hope Lane!"});
        }

        public void Handler(string test)
        { 

        } 

        public void Handler(TestMessage2Event message)
        {
            Console.WriteLine(string.Format("Received test message 2 with name {0} and address {1}", message.Name, message.Addresss));
        }
    }
}
