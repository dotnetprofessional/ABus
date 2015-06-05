using ABus.AzureServiceBus;
using ABus.Config;
using ABus.Unity;
 
namespace ABus.Sample.Client
{
    public class StartupClient: IConfigureHost
    {
        public void Configure(ConfigurationGrammar configure)
        {
            configure
                .EnsureQueueExists()
                .UseTransport<AzureBusTransport>("CustomerBC")
                .WithConnectionString(
                    "Endpoint=sb://abus-dev.servicebus.windows.net;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=uyauQw6sme25rx0EzLc/2VSWafIF6PROzdkZ9A4N918=");

            configure.Transactions.WithTransactionManager<DefaultTransactionManager>("connection string");
            configure.UseContainer(new UnityBootstraper());
        }
    }
}
