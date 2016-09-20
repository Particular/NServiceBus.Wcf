namespace NServiceBus
{
    using System;
    using System.ServiceModel;
    using Configuration.AdvanceExtensibility;
    using Features;

    /// <summary>
    /// Configuration extensions for Wcf support.
    /// </summary>
    public static class WcfEndpointConfigurationExtensions
    {
        /// <summary>
        /// Specifies a binding provider which allows to return a specific binding for a specific service type.
        /// </summary>
        /// <remarks>The default binding provider returns a new <see cref="BasicHttpBinding"/> for each service type.</remarks>
        /// <param name="configuration">The endpoint configuration.</param>
        /// <param name="provider">The binding provider.</param>
        public static void BindingProvider(this EndpointConfiguration configuration, Func<Type, BindingConfiguration> provider)
        {
            configuration.GetSettings().Set(WcfSupport.bindingProviderKey, provider);
        }

        /// <summary>
        /// Specifies a cancellation timeout provider which allows to return a specific cancellation timespan for a specific service type.
        /// </summary>
        /// <remarks>The default binding provider returns TimeSpan.FromSeconds(60) for each service type.</remarks>
        /// <param name="configuration">The endpoint configuration.</param>
        /// <param name="provider">The binding provider.</param>
        public static void CancelAfterProvider(this EndpointConfiguration configuration, Func<Type, TimeSpan> provider)
        {
            configuration.GetSettings().Set(WcfSupport.cancelAfterProviderKey, provider);
        }
    }
}