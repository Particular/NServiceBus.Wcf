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
        /// <param name="request"></param>
        /// <returns>Returns the result received from the messaging endpoint.</returns>
        [OperationContract]
        Task<TResponse> Process(TRequest request);
    }
}
