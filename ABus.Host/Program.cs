using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using ABus.AzureServiceBus;
using ABus.Contracts;
using ABus.Sample;
using ABus.Unity;
using Newtonsoft.Json;

namespace ABus.Host
{
    public class Program
    { 
        public static void Main()
        {


            var executionPath = Directory.GetCurrentDirectory();


            var consoleTracer = new ColorConsoleTraceListener();
            Trace.Listeners.Add(consoleTracer);
             
            var p = new Pipeline(new UnityBootstraper());

            p.Configure.Pipeline
                .InboundMessage.Security.Register<CustomSecurityTask>()
                .And()
                .EnsureQueueExists()
                .UseTransport<AzureBusTransport>("CustomerBC")
                .WithConnectionString("Endpoint=sb://abus-dev.servicebus.windows.net;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=uyauQw6sme25rx0EzLc/2VSWafIF6PROzdkZ9A4N918=");

            p.Configure.WithMessageEndpoint
                .UseTransport("CustomerBC").WithPattern("ABus.Sample")
                .AndAlso
                .UseTransport("CustomerBC").WithDefaultPattern();

            p.Configure.Transactions.WithTransactionManager<DefaultTransactionManager>("connection string");

            p.Start();

            //Thread.Sleep(3000);
            for (int i = 1; i < 2; i++)
            {
                Main3(i, p.GetDefaultBusInstance());

            }

            Console.ReadLine();  
        }


        public static void Main3(int count, IBus bus)
        {
            var consoleTracer = new ConsoleTraceListener();
            
            Trace.Listeners.Add(consoleTracer);

            // Create the topic
            var t = new AzureBusTransport();
            var host = new TransportDefinition
            {
                Uri = "sb://abus-dev.servicebus.windows.net",
                Credentials = "SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=uyauQw6sme25rx0EzLc/2VSWafIF6PROzdkZ9A4N918=",
                TransportType = typeof (AzureBusTransport).FullName
            };

            t.ConfigureHost(host);

            var entity = new TestMessageCommand { Name = "Sample Message", Addresss = count + " Way" };

            bus.Send(entity);

            //var json = JsonConvert.SerializeObject(entity);
            //var raw = new RawMessage { Body = Encoding.Unicode.GetBytes(json) };
            //raw.MetaData.Add(new MetaData { Name = StandardMetaData.MessageType, Value = entity.GetType().FullName });
            //var endpoint = new QueueEndpoint { Host = host.Uri, Name = "abus.sample.testmessage" };
            //t.Send(endpoint, raw);
            return;
            //var busProcess = new BusProcessHost();
            //HostFactory.Run(x =>
            //{
            //    x.Service<BusProcessHost>(s =>
            //    {
            //        s.ConstructUsing(name => busProcess);
            //        s.WhenStarted(tc => tc.Start());
            //        s.WhenStopped(tc => tc.Stop());
            //    });
            //    x.RunAsLocalSystem();

            //    x.SetDescription("ABus Service Host");
            //    x.SetDisplayName("ABus");
            //    x.SetServiceName("ABus");
            //});                                             
        }

        static MetaDataCollection CreateConfiguration()
        {
            var config = new MetaDataCollection();
            var connectionStringKey = "AzureServiceBus.ConnectionString";
            var connectionString = ConfigurationManager.AppSettings[connectionStringKey];
            config.Add(new MetaData { Name = connectionStringKey, Value = connectionString });

            return config;
        }

    }
}
