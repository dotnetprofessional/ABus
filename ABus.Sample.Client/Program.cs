using System;
using System.Text;
using ABus.AzureServiceBus;
using ABus.Contracts;
using ABus.Sample.Contracts.Payments;
using Newtonsoft.Json;
using System.Linq;
using ABus.MemoryServiceBus;

namespace ABus.Sample.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "azure":
                        MainAzure(args.Skip(1).ToArray());
                        return;
                    case "memory":
                        MainMemory(args.Skip(1).ToArray());
                        return;
                }
            }
            Console.WriteLine("Specify transport: Client.exe azure|memory");
        }

        static void MainMemory(string[] args)
        {
            var transport = new MemoryBusTransport();
            var host = new TransportDefinition
            {
                Uri = "host1",
            };
            //Run(transport, host);

            transport.ConfigureHost(host);

            QueueEndpoint topicEndpoint = new QueueEndpoint { Host = "host1", Name = "topic1" };
            transport.CreateQueue(topicEndpoint);

            transport.Subscribe(topicEndpoint, "subscription1");

            RawMessage message = new RawMessage { Body = new byte[] { 97 } };
            transport.Send(topicEndpoint, message);
        }

        static void MainAzure(string[] args)
        {
            var t = new AzureBusTransport();
            var host = new TransportDefinition
            {
                Uri = "sb://abus-dev.servicebus.windows.net",
                Credentials = "SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=uyauQw6sme25rx0EzLc/2VSWafIF6PROzdkZ9A4N918=",
            };
            Run(t, host);
        }

        static void Run(IMessageTransport t, TransportDefinition host)
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
                    SendPaymentCommandMessage(t, host);
                } while (Console.ReadLine() != "x");
            }
        }

        static void SendPaymentCommandMessage(IMessageTransport t, TransportDefinition host)
        {
            t.ConfigureHost(host);

            var entity = new MakePaymentCommand {};
            var json = JsonConvert.SerializeObject(entity);
            var raw = new RawMessage { Body = Encoding.Unicode.GetBytes(json) };
            var endpoint = new QueueEndpoint { Host = host.Uri, Name = "PaymentQueue" };
            t.Send(endpoint, raw);
        }
    }
}
