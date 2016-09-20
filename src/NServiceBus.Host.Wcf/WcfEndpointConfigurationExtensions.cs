namespace NServiceBus
{
    using Configuration.AdvanceExtensibility;

    /// <summary>
    /// Configuration extensions for Wcf support.
    /// </summary>
    public static class WcfEndpointConfigurationExtensions
    {
        /// <summary>
        /// Configuration settings for WCF.
        /// </summary>
        /// <param name="configuration">The endpoint configuration.</param>
        public static WcfSettings Wcf(this EndpointConfiguration configuration)
        {
            return new WcfSettings(configuration.GetSettings());
        }
    }
}