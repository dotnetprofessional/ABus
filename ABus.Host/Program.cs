using System;
using System.Configuration;
using System.Diagnostics;
using System.Text;
using ABus.AzureServiceBus;
using ABus.Contracts;
using ABus.Sample;
using ABus.Unity;
using Newtonsoft.Json;
using Topshelf;

namespace ABus.Host
{
    public class Program
    {
        public static void Main()
        {
            for (int i = 1; i < 2; i++)
            {
                Main3(i);
               
            }
            var consoleTracer = new ColorConsoleTraceListener();
            Trace.Listeners.Add(consoleTracer);
             
            var p = new Pipeline(new UnityBootstraper());

            p.StartupPipeline 
                .Initialize.Register("task1", typeof(InitailizePipeline3))
                .Then("task1", typeof(InitailizePipeline4))
                .And()
                .InboundMessagePipeline  
                .Security.Register("task1", typeof(InboundMessageTask));
             
            p.Start();

            Console.ReadLine();
        }


        public static void Main3(int count)
        {
            var consoleTracer = new ConsoleTraceListener();
            
            Trace.Listeners.Add(consoleTracer);

            // Create the topic
            IMessageTransport transport = new AzureBusTransport();
            var host = new TransportDefinition
            {
                Uri = "sb://abus-dev.servicebus.windows.net",
                Credentials = "SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=uyauQw6sme25rx0EzLc/2VSWafIF6PROzdkZ9A4N918=",
                TransportObsolete = typeof (AzureBusTransport)
            };

            transport.ConfigureHost(host);

            var entity = new TestMessageCommand { Name = "Sample Message", Addresss = count + " Way" };
            var json = JsonConvert.SerializeObject(entity);
            var raw = new RawMessage { Body = Encoding.Unicode.GetBytes(json) };
            raw.MetaData.Add(new MetaData{Name = StandardMetaData.MessageType, Value = entity.GetType().FullName});
            var endpoint = new QueueEndpoint { Host = host.Uri, Name = "abus.sample.testmessage" };
            transport.Send(endpoint, raw);
            return;
            var busProcess = new BusProcessHost();
            HostFactory.Run(x =>
            {
                x.Service<BusProcessHost>(s =>
                {
                    s.ConstructUsing(name => busProcess);
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("ABus Service Host");
                x.SetDisplayName("ABus");
                x.SetServiceName("ABus");
            });                                             
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
