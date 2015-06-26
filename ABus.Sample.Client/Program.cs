using System;
using System.Text;
using ABus.AzureServiceBus;
using ABus.Contracts;
using ABus.Sample.Contracts.Payments;
using Newtonsoft.Json;

namespace ABus.Sample.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Client started - press enter to send messages");
            Console.ReadLine();
            var pipeline = new Pipeline();
            var bus = pipeline.StartUsingConfigureHost();
            {
                bus.Trace.Information("About to send some messages!!!");

                do
                {
                    // Using CustomerBC
                    bus.Send(new TestMessageCommand());

                    // Using PaymentsBC
                    //bus.Send(new MakePaymentCommand());

                    // Send without using the ABus infrastructure - to simulate a third party system sending a message
                    //SendPaymentCommandMessage();
                } while (Console.ReadLine() != "x");
            }
        }

        static void SendPaymentCommandMessage()
        {
            var t = new AzureBusTransport();
            var host = new TransportDefinition
            {
                Uri = "sb://abus-dev.servicebus.windows.net",
                Credentials = "SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=uyauQw6sme25rx0EzLc/2VSWafIF6PROzdkZ9A4N918=",
            };

            t.ConfigureHost(host);

            var entity = new MakePaymentCommand {};
            var json = JsonConvert.SerializeObject(entity);
            var raw = new RawMessage { Body = Encoding.Unicode.GetBytes(json) };
            var endpoint = new QueueEndpoint { Host = host.Uri, Name = "PaymentQueue" };
            t.SendAsync(endpoint, raw).Wait();
        }
    }
}
