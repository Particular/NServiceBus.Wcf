namespace NServiceBus
{
    using System;

    interface IProvideSendOptions
    {
        Func<SendOptions> SendOptionsProvider { get; set; }
    }
}