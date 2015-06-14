using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ABus.MemoryServiceBus.ServiceBus;

namespace ABus.MemoryServiceBus.Tests
{
    [TestClass]
    public class TopicTests
    {
        [TestMethod]
        public void When_TopicDoesntExist_CreateTopic_Succeeds()
        {
            MemoryHost host = new MemoryHost(new Contracts.TransportDefinition { Uri = "host1" });
            var topic = host.CreateTopic("topic1");
            Assert.IsNotNull(topic);
            Assert.AreEqual("host1", topic.HostUri);
            Assert.AreEqual("topic1", topic.Name);
        }

        [TestMethod]
        public void When_TopicDoesntExist_TopicExists_ReturnsFalse()
        {
            MemoryHost host = new MemoryHost(new Contracts.TransportDefinition { Uri = "host1" });
            var exists = host.TopicExists("topic1");
            Assert.IsFalse(exists);
        }

        [TestMethod]
        public void When_TopicExist_TopicExists_ReturnsTrue()
        {
            MemoryHost host = new MemoryHost(new Contracts.TransportDefinition { Uri = "host1" });
            var topic = host.CreateTopic("topic1");
            var exists = host.TopicExists("topic1");
            Assert.IsTrue(exists);
        }

        [TestMethod]
        public void When_TopicExist_CreatingSameTopic_Succeeds()
        {
            MemoryHost host = new MemoryHost(new Contracts.TransportDefinition { Uri = "host1" });
            var topic = host.CreateTopic("topic1");
            var sameTopic = host.CreateTopic("topic1");
            Assert.IsNotNull(sameTopic);
            Assert.AreEqual("host1", sameTopic.HostUri);
            Assert.AreEqual("topic1", sameTopic.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void When_TopicDoesntExist_GetTopic_Throws()
        {
            MemoryHost host = new MemoryHost(new Contracts.TransportDefinition { Uri = "host1" });
            var topic = host.GetTopic("topic1");
            //var topic = host.CreateTopic("topic1");
            //var exists = host.TopicExists("topic1");
            Assert.Fail();
        }

    }
}
