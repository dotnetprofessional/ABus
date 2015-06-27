using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ABus.MemoryServiceBus.ServiceBus;

namespace ABus.MemoryServiceBus.Tests
{
    [TestClass]
    public class SubscriptionTests
    {
        [TestMethod]
        public void When_SubscriptionDoesntExist_CreateSubscription_Succeeds()
        {
            MemoryHost host = new MemoryHost(new Contracts.TransportDefinition { Uri = "host1" });
            var topic = host.CreateTopic("topic1");

            var subscription = topic.CreateSubscription("sub1", new SubscriptionOptions());
            Assert.IsNotNull(subscription);
            Assert.AreEqual("sub1", subscription.Name);
            Assert.AreEqual("topic1", subscription.TopicName);
            Assert.AreEqual("host1", subscription.HostUri);
        }

        [TestMethod]
        public void When_SubscriptionExists_CreateSameSubscription_Succeeds()
        {
            MemoryHost host = new MemoryHost(new Contracts.TransportDefinition { Uri = "host1" });
            var topic = host.CreateTopic("topic1");

            var subscription = topic.CreateSubscription("sub1", new SubscriptionOptions());
            var sameSubscription = topic.CreateSubscription("sub1", new SubscriptionOptions());
            Assert.IsNotNull(sameSubscription);
            Assert.AreEqual("sub1", sameSubscription.Name);
            Assert.AreEqual("topic1", sameSubscription.TopicName);
            Assert.AreEqual("host1", sameSubscription.HostUri);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void When_SubscriptionDoesntExist_GetSubscription_Throw()
        {
            MemoryHost host = new MemoryHost(new Contracts.TransportDefinition { Uri = "host1" });
            var topic = host.CreateTopic("topic1");

            var subscription = topic.GetSubscription("sub1");
            Assert.Fail("Topic.GetSubscription() should've thrown an exception");
        }

        [TestMethod]
        public void When_CreatingSubscription_SubscriptionsPropertyContainsNewSubscription()
        {
            MemoryHost host = new MemoryHost(new Contracts.TransportDefinition { Uri = "host1" });
            var topic = host.CreateTopic("topic1");

            topic.CreateSubscription("sub1", new SubscriptionOptions());
            Assert.AreEqual(1, topic.Subscriptions.Count);
            Subscription subscription;
            Assert.IsTrue(topic.Subscriptions.TryGetValue("sub1", out subscription));
            Assert.AreEqual("sub1", subscription.Name);
        }

    }
}
