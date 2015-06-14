using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ABus.MemoryServiceBus.ServiceBus;
using ABus.Contracts;

namespace ABus.MemoryServiceBus.Tests
{
    [TestClass]
    public class BrokeredMessageTests
    {
        [TestMethod]
        public void When_RawMessage_HasBody_BrokeredMessage_IsWellFormed()
        {
            RawMessage rawMessage = new RawMessage
            {
                MessageId = "msg1",
                Body = new byte[] { 0x1, 0x2, 0x3 },
            };
            BrokeredMessage message = new BrokeredMessage("host", "topic", rawMessage);
            Assert.AreEqual("host", message.Host);
            Assert.AreEqual("topic", message.Topic);
            Assert.AreNotSame(message.Message, rawMessage);
            Assert.AreEqual(3, message.Message.Body.Length);
            for(int i = 0; i < rawMessage.Body.Length; i++)
            {
                Assert.AreEqual(message.Message.Body[i], rawMessage.Body[i]);
            }
        }

        [TestMethod]
        public void When_RawMessage_HasProperties_BrokeredMessage_IsWellFormed()
        {
            var metadata = new MetaDataCollection();
            metadata.Add(new MetaData { Name = "name1", Value = "1" });
            metadata.Add(new MetaData { Name = "name2", Value = "2" });
            RawMessage rawMessage = new RawMessage
            {
                MessageId = "msg1",
                Body = new byte[] { 0x1, 0x2, 0x3 },
                MetaData = metadata
            };
            BrokeredMessage message = new BrokeredMessage("host", "topic", rawMessage);
            Assert.AreEqual("host", message.Host);
            Assert.AreEqual("topic", message.Topic);
            Assert.AreNotSame(message.Message, rawMessage);
            Assert.AreNotSame(message.Message.MetaData, rawMessage.MetaData);
            Assert.AreEqual(3, message.Message.Body.Length);
            Assert.AreEqual(message.Message.Body[0], rawMessage.Body[0]);
            Assert.AreEqual(message.Message.Body[1], rawMessage.Body[1]);
            Assert.AreEqual(message.Message.Body[2], rawMessage.Body[2]);

            Assert.AreEqual("name1", message.Message.MetaData[0].Name);
            Assert.AreEqual("1", message.Message.MetaData[0].Value);
            Assert.AreEqual("name2", message.Message.MetaData[1].Name);
            Assert.AreEqual("2", message.Message.MetaData[1].Value);
        }
    }
}
