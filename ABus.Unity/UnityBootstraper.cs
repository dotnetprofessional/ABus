using ABus.Contracts;
using Microsoft.Practices.Unity;

namespace ABus.Unity
{
    public class UnityBootstraper : UnityContainerAdaptor, IABusContainer
    {
        public UnityBootstraper(IUnityContainer container) : base(container)
        {
            this.RegisterTypes();
        }

        public UnityBootstraper()
        {
            this.RegisterTypes();
        }
 
        void RegisterTypes()
        {
            this.container.RegisterType<IAssemblyResolver, AssemblyResolver>(new ContainerControlledLifetimeManager());
            this.container.RegisterType<IManageTransactions, DefaultTransactionManager>();
        }
    }
}