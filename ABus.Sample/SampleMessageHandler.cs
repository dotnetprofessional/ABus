using System;
using System.Threading.Tasks;
using ABus.Contracts;
using ABus.Sample.Contracts.Payments;

namespace ABus.Sample
{
    public class SampleMessageHandler : IHandleMessage<TestMessageCommand>//, IHandleMessage<TestMessage2Event>, IHandleMessage<MakePaymentCommand>, IConfigureHandler<MakePaymentCommand>
    {
        public IBus Bus { get; set; }

        public async Task HandlerAsync(TestMessageCommand messageCommand)
        {
            //Console.WriteLine(string.Format("Received message with Id {0} and Name {1}", this.Bus.CurrentMessage.MessageId, message.Name));
            Console.WriteLine(string.Format("Received message with name {0} and address {1}", messageCommand.Name, messageCommand.Addresss));

            this.Bus.Send(new TestMessage2Event{Name = "Test Send of a message!", Addresss = "No Hope Lane!"});

            //throw new Exception("Demo Exception!");
        }

        public void Handler(string test)
        { 

        } 

        public async Task Handler(TestMessage2Event message)
        {
            Console.WriteLine(string.Format("Received test message 2 with name {0} and address {1}", message.Name, message.Addresss));
        }

        public async Task  Handler(MakePaymentCommand message)
        {
            Console.WriteLine(string.Format("Received message type {0} ", message.GetType().Name));

            this.Bus.Send(new TestMessage2Event { Name = "Payment Received!", Addresss = "No return address.." });
        }

        public void HandlerConfig(RegisteredHandler handler)
        {
            handler.SubscriptionName = "MyCustomSubscriptionName";
        }
    }
}
