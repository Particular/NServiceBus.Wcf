namespace NServiceBus.Features
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Threading.Tasks;
    using Hosting.Wcf;

    class WcfSupport : Feature
    {
        public WcfSupport()
        {
            EnableByDefault();
            Defaults(s => s.SetDefault(bindingProviderKey, new Func<Type, Tuple<Binding, string>>(t => Tuple.Create< Binding, string>(new BasicHttpBinding(), string.Empty))));
        }

        protected override void Setup(FeatureConfigurationContext context)
        {
            var conventions = context.Settings.Get<Conventions>();
            var availableTypes = context.Settings.GetAvailableTypes();
            var serviceTypes = availableTypes.SelectServiceTypes(conventions);
            var provider = context.Settings.Get<Func<Type, Tuple<Binding, string>>>(bindingProviderKey);

            context.RegisterStartupTask(new StartupTask(new WcfManager(serviceTypes, provider)));
        }

        public const string bindingProviderKey = "BindingProvider";

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