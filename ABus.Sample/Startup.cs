using ABus.AzureServiceBus;
using ABus.Config;
using ABus.Tasks.Startup;
using ABus.Unity;
 
namespace ABus.Sample
{
    public class Startup : IConfigureHost
    {
        public void Configure(ConfigurationGrammar configure)
        {
            configure.Pipeline
                .InboundMessage.Security.Register<CustomSecurityTask>()
                .And().Pipeline.Startup.Initialize.RegisterBefore<ValidateQueuesTask, CustomStartupTask>()
                .And()
                .EnsureQueueExists()
                .UseTransport<AzureBusTransport>("CustomerBC")
                .WithConnectionString(
                    "Endpoint=sb://abus-dev.servicebus.windows.net;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=uyauQw6sme25rx0EzLc/2VSWafIF6PROzdkZ9A4N918=")
                .And()
                .UseTransport<AzureBusTransport>("PaymentsBC")
                .WithConnectionString(
                    "Endpoint=sb://abus-dev.servicebus.windows.net;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=uyauQw6sme25rx0EzLc/2VSWafIF6PROzdkZ9A4N918=");

            configure.WithMessageEndpoint
                .UseTransport("PaymentsBC").WithPattern("ABus.Sample.Contracts.Payments").WithEndpoint("PaymentQueue")
                .AndAlso
                .UseTransport("CustomerBC").WithDefaultPattern();

            configure.Transactions.WithTransactionManager<DefaultTransactionManager>("connection string");
            configure.UseContainer(new UnityBootstraper());
        }
    }
}
