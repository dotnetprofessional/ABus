﻿using System;
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
            var consoleTracer = new ColorConsoleTraceListener();
            Trace.Listeners.Add(consoleTracer);

            var p = new Pipeline(new UnityBootstraper());

            p.StartupPipeline
                .Initialize.Register("task1", typeof(InitailizePipeline3))
                .Then("task1", typeof(InitailizePipeline4))
                .And()
                .InboundMessagePipeline
                .Authenticate.Register("task1", typeof(InboundMessageTask));
             
            p.Start();

            Console.ReadLine();
        }


        public static void Main2()
        {
            var consoleTracer = new ConsoleTraceListener();
            
            Trace.Listeners.Add(consoleTracer);

            // Create the topic
            var t = new AzureBusTransport();
            var host = new TransportDefinition
            {
                Uri = "sb://abus-dev.servicebus.windows.net",
                Credentials = "SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=uyauQw6sme25rx0EzLc/2VSWafIF6PROzdkZ9A4N918=",
                TransportObsolete = typeof (AzureBusTransport)
            };

            t.ConfigureHost(host);

            var entity = new TestMessageCommand { Name = "Sample Message", Addresss = "1 Way" };
            var json = JsonConvert.SerializeObject(entity);
            var raw = new RawMessage { Body = Encoding.Unicode.GetBytes(json) };
            var endpoint = new QueueEndpoint { Host = host.Uri, Name = "ABus.Sample.TestMessageCommand" };
            t.Send(endpoint, raw);

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
