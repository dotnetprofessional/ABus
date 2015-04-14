using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using ABus.AzureServiceBus;
using ABus.Contracts;
using ABus.Sample;
using Newtonsoft.Json;
using Topshelf;
using Topshelf.Hosts;
using Topshelf.Logging;

namespace ABus.Host
{
    public class Program
    {
        public static void Main()
        {
            // Create the topic
            var t = new AzureBusTransport();
            var host = new ABus.Contracts.HostDefinition
            {
                Uri = "sb://abus-dev.servicebus.windows.net",
                Credentials = "SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=uyauQw6sme25rx0EzLc/2VSWafIF6PROzdkZ9A4N918=",
                Transport = typeof (AzureBusTransport)
            };

            t.ConfigureHost(host);

            var entity = new TestMessage { Name = "Sample Message", Addresss = "1 Way" };
            var json = JsonConvert.SerializeObject(entity);
            var raw = new RawMessage { Body = System.Text.Encoding.Unicode.GetBytes(json) };
            var endpoint = new QueueEndpoint { Host = host.Uri, Name = "ABus.Sample.TestMessage" };
            t.Send(endpoint, raw);

            HostFactory.Run(x =>
            {
                x.Service<BusProcessHost>(s =>
                {
                    s.ConstructUsing(name => new BusProcessHost());
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
