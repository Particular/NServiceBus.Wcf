namespace NServiceBus.Hosting.Wcf
{
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;

    class MessageSessionInspector
        : IDispatchMessageInspector
    {
        public MessageSessionInspector(IMessageSession session)
        {
            messageSession = session;
        }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var instance = instanceContext.GetServiceInstance(request);
            var sessionProvider = instance as IProvideMessageSession;
            if (sessionProvider != null)
            {
                sessionProvider.Session = messageSession;
            }
            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
        }

        IMessageSession messageSession;
    }
}