namespace NServiceBus.Features
{
    using System;
    using System.ServiceModel;
    using System.Threading.Tasks;
    using Hosting.Wcf;

    class WcfSupport : Feature
    {
        public WcfSupport()
        {
            EnableByDefault();
            Defaults(s => s.SetDefault(bindingProviderKey, new Func<Type, BindingConfiguration>(t => new BindingConfiguration(new BasicHttpBinding()))));
            Defaults(s => s.SetDefault(cancelAfterProviderKey, new Func<Type, TimeSpan>(t => TimeSpan.FromSeconds(60))));
        }

        protected override void Setup(FeatureConfigurationContext context)
        {
            var conventions = context.Settings.Get<Conventions>();
            var availableTypes = context.Settings.GetAvailableTypes();
            var serviceTypes = availableTypes.SelectServiceTypes(conventions);

            var bindingProvider = context.Settings.Get<Func<Type, BindingConfiguration>>(bindingProviderKey);
            var cancelAfterProvider = context.Settings.Get<Func<Type, TimeSpan>>(cancelAfterProviderKey);

            context.RegisterStartupTask(new StartupTask(new WcfManager(serviceTypes, bindingProvider, cancelAfterProvider)));
        }

        public const string bindingProviderKey = "BindingProvider";
        public const string cancelAfterProviderKey = "CancelAfterProvider";

        class StartupTask : FeatureStartupTask
        {
            public StartupTask(WcfManager wcfManager)
            {
                this.wcfManager = wcfManager;
            }

            protected override Task OnStart(IMessageSession session)
            {
                return wcfManager.Startup(session);
            }

            protected override Task OnStop(IMessageSession session)
            {
                return wcfManager.Shutdown();
            }

            WcfManager wcfManager;
        }
    }
}