namespace NServiceBus
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
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
        public static void BindingProvider(this EndpointConfiguration configuration, Func<Type, Tuple<Binding, string>> provider)
        {
            configuration.GetSettings().Set(WcfSupport.bindingProviderKey, provider);
        }
    }
}