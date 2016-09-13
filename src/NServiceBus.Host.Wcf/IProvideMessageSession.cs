namespace NServiceBus
{
    interface IProvideMessageSession
    {
        IMessageSession Session { get; set; }
    }
}