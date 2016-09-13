namespace NServiceBus.Hosting.Wcf
{
    using System;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Configuration;

    class WcfServiceHost : ServiceHost
    {
        IMessageSession session;

        public WcfServiceHost(Type t, IMessageSession session)
            : base(t)
        {
            this.session = session;
        }

        /// <summary>
        /// Adds the given endpoint unless its already configured in app.config
        /// </summary>
        public void AddDefaultEndpoint(Type contractType, Binding binding, string address)
        {
            var serviceModel = ServiceModelSectionGroup.GetSectionGroup(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None));

            if (serviceModel == null)
                throw new InvalidOperationException("No service model section found in config");

            var endpointAlreadyConfigured = false;

            foreach (ServiceElement se in serviceModel.Services.Services)
            {
                if (se.Name == Description.ConfigurationName)
                {
                    foreach (ServiceEndpointElement endpoint in se.Endpoints)
                    {
                        if (endpoint.Contract == contractType.FullName && endpoint.Address.OriginalString == address)
                            endpointAlreadyConfigured = true;
                    }
                }
            }

            foreach (var cd in ImplementedContracts.Values)
            {
                cd.Behaviors.Add(new MessageSessionInstanceProviderContractBehavior(session));
            }

            if (!endpointAlreadyConfigured)
            {
                AddServiceEndpoint(contractType, binding, address);
            }
        }
    }
}