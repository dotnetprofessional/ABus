using System.Configuration;
using System.Text;
using System.Threading;
using ABus.Contracts;
using FluentAssertions;
using LiveSpec.Extensions.MSpec;
using Machine.Specifications;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace ABus.AzureServiceBus.Test.Integration
{
    [Subject("An Azure Service Bus transport used by the ABus library")]
    public class TransportSpec
    {
        [Given(@"Given the queue
                '''
                transport-spec-queue
                '''")]
        public class When_sending_a_message_to_an_existing_queue
        {
            static LiveDocScenario Scenario;
            static IMessageTransport Transport;
            Establish given = () =>
            {
                Scenario = new LiveDocScenario(typeof (When_sending_a_message_to_an_existing_queue));
                Transport = new AzureBusTransport();
                // Establish the queue
                CreateNewQueue(Scenario.Given.DocString);
            };

            Because when = () =>
            {
                var msg = new RawMessage {Body = UTF8Encoding.UTF8.GetBytes("This is a test")};
                Transport.Send(Scenario.Given.DocString, msg);
            };

            It should_arrive_on_the_named_queue = () => ReadQueue(Scenario.Given.DocString).MessageId.Should().NotBeNullOrEmpty();

        }

        [Given(@"Given the queue
                '''
                transport-spec-queue
                '''")]
        public class When_subscribing_to_a_queue_messages
        {
            static LiveDocScenario Scenario;
            static IMessageTransport Transport;
            static RawMessage ReceivedMessage;

            Establish given = () =>
            {
                Scenario = new LiveDocScenario(typeof(When_sending_a_message_to_an_existing_queue));
                Transport = new AzureBusTransport(CreateConfiguration());

                // Setup the event handler to set the ReceivedMessage when a message is returned from the transport
                Transport.MessageReceived += (s, e) =>
                {
                    ReceivedMessage = e;
                };

                // Establish the queue
                CreateNewQueue(Scenario.Given.DocString);

                // Put a message on the queu ready to be received
                var bm = new BrokeredMessage("This is a test");
                
                SendToQueue(Scenario.Given.DocString, bm);
            };

            Because when = () =>
            {
                Transport.SubscribeAsync(Scenario.Given.DocString, "all");                
            };

            It should_receive_messages_sent_to_the_queue = () =>
            {
                Thread.Sleep(1000);
                ReceivedMessage.MessageId.Should().NotBeNull();
            };

        }

        static void CreateNewQueue(string queue)
        {
            var connectionString = ConfigurationManager.AppSettings["AzureServiceBus.ConnectionString"];
            var ns = NamespaceManager.CreateFromConnectionString(connectionString);
            if (ns.TopicExists(queue))
                return;
                    //ns.DeleteTopic(queue);

            ns.CreateTopic(queue);
            ns.CreateSubscription(queue, "all");
        }

        static BrokeredMessage ReadQueue(string queue)
        {
            var connectionString = ConfigurationManager.AppSettings["AzureServiceBus.ConnectionString"];

            var subscriptionClient = SubscriptionClient.CreateFromConnectionString(connectionString, queue, "all");
            var msg = subscriptionClient.Receive();
            return msg;
        }

        static void SendToQueue(string queue, BrokeredMessage msg)
        {
            var connectionString = ConfigurationManager.AppSettings["AzureServiceBus.ConnectionString"];

            var topicClient = TopicClient.CreateFromConnectionString(connectionString, queue);
            topicClient.Send(msg);
        }

        static MetaDataCollection CreateConfiguration()
        {
            var config = new MetaDataCollection();
            var connectionStringKey = "AzureServiceBus.ConnectionString";
            var connectionString = ConfigurationManager.AppSettings[connectionStringKey];
            config.Add(new MetaData{Name = connectionStringKey, Value = connectionString});

            return config;
        }
    }
}
