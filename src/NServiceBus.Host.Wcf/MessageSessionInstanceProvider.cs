namespace NServiceBus.Hosting.Wcf
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;

    class MessageSessionInstanceProvider : IInstanceProvider
    {
        IMessageSession session;
        IInstanceProvider instanceProvider;

        public MessageSessionInstanceProvider(IMessageSession session, IInstanceProvider decoratedProvider)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            instanceProvider = decoratedProvider;
            this.session = session;
        }

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            var instance = instanceProvider.GetInstance(instanceContext, message);
            var sessionProvider = instance as IProvideMessageSession;
            if (sessionProvider != null)
            {
                sessionProvider.Session = session;
            }
            return instance;
        }

        public object GetInstance(InstanceContext instanceContext)
        {
            var instance = instanceProvider.GetInstance(instanceContext);
            var sessionProvider = instance as IProvideMessageSession;
            if (sessionProvider != null)
            {
                sessionProvider.Session = session;
            }
            return instance;
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            var sessionProvider = instance as IProvideMessageSession;
            if (sessionProvider != null)
            {
                sessionProvider.Session = null;
            }
            instanceProvider.ReleaseInstance(instanceContext, instance);
        }
    }
}