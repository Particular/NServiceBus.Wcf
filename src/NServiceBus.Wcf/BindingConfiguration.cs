namespace NServiceBus
{
    using System;
    using System.ServiceModel.Channels;

    /// <summary>
    /// Configuration object for bindings and addresses.
    /// </summary>
    public class BindingConfiguration
    {
        /// <summary>
        /// Creates an instance of a configuration with a specified binding.
        /// </summary>
        /// <param name="binding">The binding to be used.</param>
        public BindingConfiguration(Binding binding)
            : this(binding, new Uri(string.Empty, UriKind.RelativeOrAbsolute))
        {
        }

        /// <summary>
        /// Creates an instance of a configuration with a specified binding and address.
        /// </summary>
        /// <param name="binding">The binding to be used.</param>
        /// <param name="address">The address to be used.</param>
        public BindingConfiguration(Binding binding, Uri address)
        {
            Binding = binding;
            Address = address;
        }

        /// <summary>
        /// The binding.
        /// </summary>
        public Binding Binding { get; }

        /// <summary>
        /// The relative or absolute address.
        /// </summary>
        public Uri Address { get; }
    }
}