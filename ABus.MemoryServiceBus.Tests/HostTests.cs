using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ABus.Contracts;

namespace ABus.MemoryServiceBus.Tests
{
    [TestClass]
    public class HostTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var transport = new MemoryBusTransport();
            var host = new TransportDefinition
            {
                Uri = "host1",
            };

            transport.ConfigureHost(host);

            QueueEndpoint topicEndpoint = new QueueEndpoint { Host = "host1", Name = "topic1" };
            transport.CreateQueue(topicEndpoint);

            transport.Subscribe(topicEndpoint, "subscription");

            RawMessage message = new RawMessage { Body = new byte[] { 97 } };
            transport.Send(topicEndpoint, message);
        }
    }
}
