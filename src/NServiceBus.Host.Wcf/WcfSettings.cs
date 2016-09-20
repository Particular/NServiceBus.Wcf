namespace NServiceBus
{
    using System;
    using System.ServiceModel;
    using Configuration.AdvanceExtensibility;
    using Features;
    using Settings;

    /// <summary>
    /// Exposes the WCF settings.
    /// </summary>
    public class WcfSettings : ExposeSettings
    {
        internal WcfSettings(SettingsHolder settings) : base(settings)
        {
        }

        /// <summary>
        /// Specifies a binding provider which allows to return a specific binding for a specific service type.
        /// </summary>
        /// <remarks>The default binding provider returns a new <see cref="BasicHttpBinding" /> for each service type.</remarks>
        /// <param name="provider">The binding provider.</param>
        public WcfSettings Binding(Func<Type, BindingConfiguration> provider)
        {
            this.GetSettings().Set(WcfSupport.bindingProviderKey, provider);
            return this;
        }

        /// <summary>
        /// Specifies a cancellation timeout provider which allows to return a specific cancellation timespan for a specific
        /// service type.
        /// </summary>
        /// <remarks>The default binding provider returns TimeSpan.FromSeconds(60) for each service type.</remarks>
        /// <param name="provider">The binding provider.</param>
        public WcfSettings CancelAfter(Func<Type, TimeSpan> provider)
        {
            this.GetSettings().Set(WcfSupport.cancelAfterProviderKey, provider);
            return this;
        }
    }
}