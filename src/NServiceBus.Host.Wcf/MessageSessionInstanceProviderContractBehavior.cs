namespace NServiceBus.Hosting.Wcf
{
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;

    class MessageSessionInstanceProviderContractBehavior : IContractBehavior
    {
        IMessageSession session;

        public MessageSessionInstanceProviderContractBehavior(IMessageSession session)
        {
            this.session = session;
        }

        public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, DispatchRuntime dispatchRuntime)
        {
            dispatchRuntime.InstanceProvider = new MessageSessionInstanceProvider(session, dispatchRuntime.InstanceProvider);
        }

        public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        {
        }
    }
}