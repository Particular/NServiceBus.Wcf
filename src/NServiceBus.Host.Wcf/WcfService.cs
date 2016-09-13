namespace NServiceBus
{
    using System.ServiceModel;
    using System.Threading.Tasks;

    /// <summary>
    /// Generic WCF service for exposing a messaging endpoint.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public abstract class WcfService<TRequest, TResponse> : IWcfService<TRequest, TResponse>, IProvideMessageSession
    {
        IProvideMessageSession Provider => this;

        /// <inheritdoc />
        IMessageSession IProvideMessageSession.Session { get; set; }

        Task<TResponse> IWcfService<TRequest, TResponse>.Process(TRequest request)
        {
            var sendOptions = new SendOptions();
            sendOptions.RouteToThisEndpoint();

            return Provider.Session.Request<TResponse>(request, sendOptions);
        }
    }
}