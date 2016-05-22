using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ABus.Contracts;
using ABus.MemoryServiceBus.ServiceBus;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace ABus.MemoryServiceBus.Tests
{
    [TestClass]
    public class MemoryBusTransportTests
    {
        [TestMethod]
        public void When_SubcribedAndMessageSent_SameMessageIsReceived()
        {
            ManualResetEventSlim messageReceivedEvent = new ManualResetEventSlim(false, 500);
            var transport = new MemoryBusTransport();
            var host = new TransportDefinition
            {
                Uri = "host1",
                EnableAuditing = false
            };
            RawMessage receivedMessage = null;
            transport.MessageReceived += (sender, args) => { receivedMessage = args; messageReceivedEvent.Set(); };

            transport.ConfigureHost(host);

            QueueEndpoint topicEndpoint = new QueueEndpoint { Host = "host1", Name = "topic1" };
            transport.CreateQueue(topicEndpoint);

            transport.Subscribe(topicEndpoint, "subscription");

            RawMessage message = new RawMessage { Body = new byte[] { 97 } };
            transport.Send(topicEndpoint, message);

            if (!messageReceivedEvent.Wait(200))
                Assert.Fail("Message not received within 200ms");

            Assert.IsNotNull(receivedMessage);
            Assert.AreEqual(1, receivedMessage.Body.Length);
            Assert.AreEqual(97, receivedMessage.Body[0]);
        }

        [TestMethod]
        public void When_AuditEnabled_MessageIsReceivedAndAudit()
        {
            ManualResetEventSlim twoMessagesReceivedEvent = new ManualResetEventSlim(false, 500);
            var transport = new MemoryBusTransport();
            var host = new TransportDefinition
            {
                Uri = "host1",
                EnableAuditing = true
            };
            ConcurrentBag<RawMessage> messagesReceived = new ConcurrentBag<RawMessage>();
            transport.MessageReceived += (sender, args) => { messagesReceived.Add(args); if (messagesReceived.Count >= 2) twoMessagesReceivedEvent.Set(); };

            transport.ConfigureHost(host);

            QueueEndpoint topicEndpoint = new QueueEndpoint { Host = "host1", Name = "topic1" };
            transport.CreateQueue(topicEndpoint);

            transport.Subscribe(topicEndpoint, "subscription");

            QueueEndpoint auditEndpoint = new QueueEndpoint { Host = "host1", Name = host.AuditQueue };
            transport.Subscribe(auditEndpoint, "auditorium");

            RawMessage message = new RawMessage { Body = new byte[] { 97 } };
            transport.Send(topicEndpoint, message);

            if (!twoMessagesReceivedEvent.Wait(200))
                Assert.Fail("Messages not received within 200ms");

            var messages = messagesReceived.ToArray();
            Assert.AreEqual(2, messages.Length);
            Assert.AreEqual(1, messages[0].Body.Length);
            Assert.AreEqual(97, messages[0].Body[0]);
            Assert.AreEqual(1, messages[1].Body.Length);
            Assert.AreEqual(97, messages[1].Body[0]);
        }
    }
}
