namespace NServiceBus
{
    /// <summary>
    /// Provides access to the message session.
    /// </summary>
    public interface IProvideMessageSession
    {
        /// <summary>
        /// The message session.
        /// </summary>
        IMessageSession Session { get; set; }
    }
}