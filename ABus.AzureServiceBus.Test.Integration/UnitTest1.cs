using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ABus.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace ABus.AzureServiceBus.Test.Integration
{
   // [TestClass]
    public class AzureServiceBusTest
    {
        //[TestMethod]
        public void When_sending_a_message_it_is_sent_to_the_correct_topic()
        {
            var config = new MetaDataCollection();
            var connectionStringKey = "AzureServiceBus.ConnectionString";
            var connectionString = ConfigurationManager.AppSettings[connectionStringKey];
            config.Add(new MetaData{Name = connectionStringKey, Value = connectionString});
            var bus = new AzureServiceBus.AzureBusTransport();

            var messages = new List<RawMessage>();
            for (int i = 0; i < 100; i++)
            {
                var entity = new TestMessage {Name = "Unit Test - " + i};
                var json = JsonConvert.SerializeObject(entity);
                var jsonBytes = System.Text.Encoding.Unicode.GetBytes(json);
                var msg = new RawMessage { Body =  jsonBytes};
                messages.Add(msg);
            }
            bus.Send(new QueueEndpoint{Host = "", Name = "unit-test"}, messages);
        }

        //[TestMethod]
        public void When_subscribing_to_a_message_type_messages_are_received()
        {
            var config = new MetaDataCollection();
            var connectionStringKey = "AzureServiceBus.ConnectionString";
            var connectionString = ConfigurationManager.AppSettings[connectionStringKey];
            config.Add(new MetaData { Name = connectionStringKey, Value = connectionString });
            var transport = new AzureServiceBus.AzureBusTransport(config);

            transport.MessageReceived += transport_MessageReceived;
            var t = transport.SubscribeAsync(new QueueEndpoint { Host = "", Name = "unit-test" }, "audit");
            var t1 = transport.SubscribeAsync(new QueueEndpoint { Host = "", Name = "unit-test" }, "sub1");

            // have the test wait for a message to arrive!?
            Thread.Sleep(5000);
        }

        void transport_MessageReceived(object sender, RawMessage e)
        {
            Debug.WriteLine(string.Format("Received from subscription: {0} messageId {1}", sender.ToString(), e.MessageId));
            //var msg = e.Body;
        }
    }
}
