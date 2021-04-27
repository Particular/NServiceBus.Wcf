#pragma warning disable PS0018 // A task-returning method should have a CancellationToken parameter unless it has a parameter implementing ICancellableContext

namespace NServiceBus
{
    using System.ServiceModel;
    using System.Threading.Tasks;

    /// <summary>
    /// Service interface for a generic WCF adapter to a messaging endpoint.
    /// </summary>
    [ServiceContract(Namespace = "http://nservicebus.com")]
    public interface IWcfService<in TRequest, TResponse>
    {
        /// <summary>
        /// Sends the message to the messaging endpoint.
        /// </summary>
        /// <param name="request">The message to be sent.</param>
        /// <returns>Returns the result received from the messaging endpoint.</returns>
        [OperationContract]
        Task<TResponse> Process(TRequest request);
    }
}

#pragma warning restore PS0018 // A task-returning method should have a CancellationToken parameter unless it has a parameter implementing ICancellableContext
