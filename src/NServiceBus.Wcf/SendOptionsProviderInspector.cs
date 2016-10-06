namespace NServiceBus.Hosting.Wcf
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;

    class SendOptionsProviderInspector
        : IDispatchMessageInspector
    {
        public SendOptionsProviderInspector(Func<SendOptions> sendOptionsProvider)
        {
            this.sendOptionsProvider = sendOptionsProvider;
        }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var instance = instanceContext.GetServiceInstance(request);
            var optionsProvider = instance as IProvideSendOptions;
            if (optionsProvider != null)
            {
                optionsProvider.SendOptionsProvider = sendOptionsProvider;
            }
            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
        }

        Func<SendOptions> sendOptionsProvider;
    }
}