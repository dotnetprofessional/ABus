using System;
using System.Collections.Generic;
using ABus.Contracts;
using Microsoft.Practices.ServiceLocation;
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
        }
    }

    public class UnityContainerAdaptor : ServiceLocatorImplBase
    {
        protected IUnityContainer container;

        public UnityContainerAdaptor():this(new UnityContainer())
        {
            
        }

        public UnityContainerAdaptor(IUnityContainer container)
        {
            this.container = container;
        }

        /// <summary>
        /// When implemented by inheriting classes, this method will do the actual work of resolving
        /// the requested service instance.
        /// </summary>
        /// <param name="serviceType">Type of instance requested.</param>
        /// <param name="key">Name of registered service you want. May be null.</param>
        /// <returns>
        /// The requested service instance.
        /// </returns>
        protected override object DoGetInstance(Type serviceType, string key)
        {
            return this.container.Resolve(serviceType, key);
        } 

        /// <summary>
        ///  When implemented by inheriting classes, this method will do the actual work of
        ///  resolving all the requested service instances.
        /// </summary>
        /// <param name="serviceType">Type of service requested.</param>
        /// <returns>
        /// Sequence of service instance objects.
        /// </returns>
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return this.container.ResolveAll(serviceType);
        }
    }
}
