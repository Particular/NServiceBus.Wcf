namespace NServiceBus.Hosting.Wcf
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.Threading.Tasks;

    /// <summary>
    ///     Enable users to expose messages as WCF services
    /// </summary>
    class WcfManager
    {
        public WcfManager(IEnumerable<Type> serviceTypes, Func<Type, BindingConfiguration> bindingProvider, Func<Type, TimeSpan> cancelAfterProvider, Func<Type, Func<SendOptions>> routeProvider)
        {
            this.cancelAfterProvider = cancelAfterProvider;
            this.routeProvider = routeProvider;
            this.bindingProvider = bindingProvider;
            this.serviceTypes = serviceTypes;
        }

        /// <summary>
        ///     Starts a <see cref="ServiceHost" /> for each found service. Defaults to <see cref="BasicHttpBinding" /> if
        ///     no user specified binding is found
        /// </summary>
        public async Task Startup(IMessageSession session)
        {
            foreach (var serviceType in serviceTypes)
            {
                var cancelAfter = cancelAfterProvider(serviceType);
                var optionsProvider = routeProvider(serviceType);
                var host = new WcfServiceHost(serviceType, session, cancelAfter, optionsProvider);

                var bindingAndAddress = bindingProvider(serviceType);
                host.AddDefaultEndpoint(GetContractType(serviceType), bindingAndAddress.Binding, bindingAndAddress.Address);
                hosts.Add(host);

                await Task.Factory.FromAsync((cbl, state) => host.BeginOpen(cbl, state), t => host.EndOpen(t), null).ConfigureAwait(false);
            }
        }

        /// <summary>
        ///     Shuts down the service hosts
        /// </summary>
        public async Task Shutdown()
        {
            foreach (var host in hosts)
            {
                await Task.Factory.FromAsync((cbl, state) => host.BeginClose(cbl, state), t => host.EndClose(t), null).ConfigureAwait(false);
            }
        }

        static Type GetContractType(Type t)
        {
            var args = t.BaseType.GetGenericArguments();

            return typeof(IWcfService<,>).MakeGenericType(args[0], args[1]);
        }

        readonly List<ServiceHost> hosts = new List<ServiceHost>();
        IEnumerable<Type> serviceTypes;
        Func<Type, BindingConfiguration> bindingProvider;
        Func<Type, TimeSpan> cancelAfterProvider;
        readonly Func<Type, Func<SendOptions>> routeProvider;
    }
}