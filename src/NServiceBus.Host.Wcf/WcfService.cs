namespace NServiceBus
{
    using System;
    using System.ServiceModel;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Generic WCF service for exposing a messaging endpoint.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public abstract class WcfService<TRequest, TResponse> : IWcfService<TRequest, TResponse>, IProvideMessageSession, IProvideCancellationSupport
    {
        IProvideMessageSession SessionProvider => this;

        IProvideCancellationSupport CancelProvider => this;

        /// <inheritdoc />
        IMessageSession IProvideMessageSession.Session { get; set; }

        /// <inheritdoc />
        TimeSpan IProvideCancellationSupport.CancelAfter { get; set; }

        async Task<TResponse> IWcfService<TRequest, TResponse>.Process(TRequest request)
        {
            using (var cts = new CancellationTokenSource(CancelProvider.CancelAfter))
            {
                var sendOptions = new SendOptions();
                sendOptions.RouteToThisEndpoint();

                try
                {
                    return await SessionProvider.Session.Request<TResponse>(request, sendOptions, cts.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    throw new FaultException($"The request was cancelled after { CancelProvider.CancelAfter } because no response was received.");
                }
            }
        }
    }
}