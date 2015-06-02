using ABus.AzureServiceBus;
using ABus.Config;
using ABus.Unity;

namespace ABus.Sample
{
    class Startup: IConfigureHost
    {
        public void Configure(ConfigurationGrammar configure)
        {
            configure.Pipeline
                .InboundMessage.Security.Register<CustomSecurityTask>()
                .And()
                .EnsureQueueExists()
                .UseTransport<AzureBusTransport>("CustomerBC")
                .WithConnectionString(
                    "Endpoint=sb://abus-dev.servicebus.windows.net;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=uyauQw6sme25rx0EzLc/2VSWafIF6PROzdkZ9A4N918=");

            configure.WithMessageEndpoint
                .UseTransport("CustomerBC").WithPattern("ABus.Sample")
                .AndAlso
                .UseTransport("CustomerBC").WithDefaultPattern();

            configure.Transactions.WithTransactionManager<DefaultTransactionManager>("connection string");
            configure.UseContainer(new UnityBootstraper());
        }
    }
}
