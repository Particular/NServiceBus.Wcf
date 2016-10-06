namespace NServiceBus.Hosting.Wcf
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;

    class CancelAfterInspector
        : IDispatchMessageInspector
    {
        public CancelAfterInspector(TimeSpan cancelAfter)
        {
            this.cancelAfter = cancelAfter;
        }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var instance = instanceContext.GetServiceInstance(request);
            var cancellationProvider = instance as IProvideCancellationSupport;
            if (cancellationProvider != null)
            {
                cancellationProvider.CancelAfter = cancelAfter;
            }
            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
        }

        TimeSpan cancelAfter;
    }
}