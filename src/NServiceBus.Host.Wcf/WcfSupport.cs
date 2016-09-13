namespace NServiceBus.Features
{
    using System.Threading.Tasks;
    using Hosting.Wcf;

    class WcfSupport : Feature
    {
        public WcfSupport()
        {
            EnableByDefault();
        }

        protected override void Setup(FeatureConfigurationContext context)
        {
            var conventions = context.Settings.Get<Conventions>();
            var availableTypes = context.Settings.GetAvailableTypes();
            var serviceTypes = availableTypes.SelectServiceTypes(conventions);

            context.RegisterStartupTask(new StartupTask(new WcfManager(serviceTypes)));
        }

        class StartupTask : FeatureStartupTask
        {
            WcfManager wcfManager;

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
        }
    }
}