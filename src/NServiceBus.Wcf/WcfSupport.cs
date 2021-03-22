namespace NServiceBus.Features
{
    using System;
    using System.ServiceModel;
    using System.Threading;
    using System.Threading.Tasks;
    using Hosting.Wcf;

    class WcfSupport : Feature
    {
        public WcfSupport()
        {
            EnableByDefault();
            Defaults(s => s.SetDefault(BindingProviderKey, new Func<Type, BindingConfiguration>(t => new BindingConfiguration(new BasicHttpBinding()))));
            Defaults(s => s.SetDefault(CancelAfterProviderKey, new Func<Type, TimeSpan>(t => TimeSpan.FromSeconds(60))));
            Defaults(s => s.SetDefault(RouteProviderKey, new Func<Type, Func<SendOptions>>(t => () =>
            {
                var sendOptions = new SendOptions();
                sendOptions.RouteToThisEndpoint();
                return sendOptions;
            })));
        }

        protected override void Setup(FeatureConfigurationContext context)
        {
            var conventions = context.Settings.Get<Conventions>();
            var availableTypes = context.Settings.GetAvailableTypes();
            var serviceTypes = availableTypes.SelectServiceTypes(conventions);

            var bindingProvider = context.Settings.Get<Func<Type, BindingConfiguration>>(BindingProviderKey);
            var cancelAfterProvider = context.Settings.Get<Func<Type, TimeSpan>>(CancelAfterProviderKey);
            var sendOptionsProvider = context.Settings.Get<Func<Type, Func<SendOptions>>>(RouteProviderKey);

            context.RegisterStartupTask(new StartupTask(new WcfManager(serviceTypes, bindingProvider, cancelAfterProvider, sendOptionsProvider)));
        }

        public const string BindingProviderKey = "BindingProvider";
        public const string CancelAfterProviderKey = "CancelAfterProvider";
        public const string RouteProviderKey = "RouteProvider";

        class StartupTask : FeatureStartupTask
        {
            public StartupTask(WcfManager wcfManager)
            {
                this.wcfManager = wcfManager;
            }

            protected override Task OnStart(IMessageSession session, CancellationToken cancellationToken)
            {
                return wcfManager.Startup(session);
            }

            protected override Task OnStop(IMessageSession session, CancellationToken cancellationToken)
            {
                return wcfManager.Shutdown();
            }

            WcfManager wcfManager;
        }
    }
}