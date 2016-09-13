namespace NServiceBus
{
    using System;
    using System.ServiceModel;
    using System.Threading.Tasks;

    /// <summary>
    /// Generic WCF service for exposing a messaging endpoint.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public abstract class WcfService<TRequest, TResponse> : IWcfService<TRequest, TResponse>
    {
        static WcfService()
        {
            // TODO: remove this?
            if (!typeof(TResponse).IsEnum)
                throw new InvalidOperationException(typeof(TResponse).FullName + " must be an enum representing error codes returned by the server.");
        }

        Task<TResponse> IWcfService<TRequest, TResponse>.Process(TRequest request)
        {
            var sendOptions = new SendOptions();
            sendOptions.RouteToThisEndpoint();

            return Session.Request<TResponse>(request, sendOptions);
        }

        /// <inheritdoc />
        public IMessageSession Session { get; set; }
    }
}