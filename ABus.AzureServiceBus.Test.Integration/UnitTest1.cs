using System;
using System.Configuration;
using ABus.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ABus.AzureServiceBus.Test.Integration
{
    [TestClass]
    public class AzureServiceBusTest
    {
        [TestMethod]
        public void When_sending_a_message_it_is_sent_to_the_correct_topic()
        {
            var config = new MetaDataCollection();
            var connectionStringKey = "AzureServiceBus.ConnectionString";
            var connectionString = ConfigurationManager.AppSettings[connectionStringKey];
            config.Add(new MetaData{Name = connectionStringKey, Value = connectionString});
            var bus = new AzureServiceBus.AzureBusTransport(config);
        }
    }
}
