using System;
using System.Text;
using ABus.AzureServiceBus;
using ABus.Contracts;
using ABus.Sample.Contracts.Payments;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
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
                Uri = "host 1",
            };
            //Run(transport, host);

            transport.ConfigureHost(host);

            QueueEndpoint topicEndpoint = new QueueEndpoint { Host = "host 1", Name = "topic 1" };
            transport.CreateQueue(topicEndpoint);
            QueueEndpoint topic2Endpoint = new QueueEndpoint { Host = "host 1", Name = "topic 2" };
            transport.CreateQueue(topic2Endpoint);

            transport.Subscribe(topicEndpoint, "sub 1 1");
            transport.Subscribe(topicEndpoint, "sub 1 2");
            transport.Subscribe(topic2Endpoint, "sub 2 1");
            transport.Subscribe(topic2Endpoint, "sub 2 2");

            const string help = "x, Esc - exit\r\n"
                + "s   - send message\r\n"
                + "p   - publish message\r\n"
                + "h,? - help message\r\n";
            ConsoleKeyInfo key;
            int messageNo = 0;
            Console.WriteLine(help);
            do
            {
                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.S)
                {
                    RawMessage message = new RawMessage { Body = Encoding.Unicode.GetBytes("message" + (++messageNo).ToString()) };
                    Console.WriteLine("Sending message " + messageNo);
                    transport.Send(topicEndpoint, message);
                }
                else if (key.Key == ConsoleKey.D)
                {
                    List<RawMessage> messages = new List<RawMessage>();
                    int startMessageNo = messageNo;
                    for (int i = 0; i < 20; i++)
                        messages.Add(new RawMessage { Body = Encoding.Unicode.GetBytes("message" + (++messageNo).ToString()) });
                    Console.WriteLine("Sending messages " + startMessageNo + " to " + messageNo);
                    transport.Send(topicEndpoint, messages);
                }
                else if (key.Key == ConsoleKey.P)
                {
                    RawMessage message = new RawMessage { Body = Encoding.Unicode.GetBytes("message" + (++messageNo).ToString()) };
                    Console.WriteLine("Sending message " + messageNo);
                    transport.Send(topicEndpoint, message);
                }
                else if (key.Key == ConsoleKey.E)
                {
                    List<RawMessage> messages = new List<RawMessage>();
                    int startMessageNo = messageNo;
                    for (int i = 0; i < 20; i++)
                        messages.Add(new RawMessage { Body = Encoding.Unicode.GetBytes("message" + (++messageNo).ToString()) });
                    Console.WriteLine("Sending messages " + startMessageNo + " to " + messageNo);
                    transport.Send(topic2Endpoint, messages);
                }
                else if (key.Key == ConsoleKey.R)
                {
                    RawMessage message = new RawMessage { Body = Encoding.Unicode.GetBytes("message" + (++messageNo).ToString()) };
                    Console.WriteLine("Sending message " + messageNo);
                    transport.Send(topic2Endpoint, message);
                }
                else if (key.Key == ConsoleKey.H || key.KeyChar == '?')
                {
                    Console.WriteLine(help);
                }

            }
            while (key.Key != ConsoleKey.Escape && key.Key != ConsoleKey.X);
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
