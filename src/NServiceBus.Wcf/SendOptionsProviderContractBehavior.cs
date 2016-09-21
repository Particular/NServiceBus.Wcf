namespace NServiceBus.Hosting.Wcf
{
    using System;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;

    class SendOptionsProviderContractBehavior : IContractBehavior
    {
        public SendOptionsProviderContractBehavior(Func<SendOptions> sendOptionsProvider)
        {
            this.sendOptionsProvider = sendOptionsProvider;
        }

        public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        {
        }

        public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, DispatchRuntime dispatchRuntime)
        {
            dispatchRuntime.MessageInspectors.Add(new SendOptionsProviderInspector(sendOptionsProvider));
        }

        public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        Func<SendOptions> sendOptionsProvider;
    }
}