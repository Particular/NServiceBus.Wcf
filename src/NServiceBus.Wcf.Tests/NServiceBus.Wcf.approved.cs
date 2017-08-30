﻿[assembly: System.Runtime.CompilerServices.InternalsVisibleToAttribute(@"NServiceBus.Wcf.Tests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100dde965e6172e019ac82c2639ffe494dd2e7dd16347c34762a05732b492e110f2e4e2e1b5ef2d85c848ccfb671ee20a47c8d1376276708dc30a90ff1121b647ba3b7259a6bc383b2034938ef0e275b58b920375ac605076178123693c6c4f1331661a62eba28c249386855637780e3ff5f23a6d854700eaa6803ef48907513b92")]
[assembly: System.Runtime.Versioning.TargetFrameworkAttribute(".NETFramework,Version=v4.5.2", FrameworkDisplayName=".NET Framework 4.5.2")]

namespace NServiceBus
{
    
    public class BindingConfiguration
    {
        public BindingConfiguration(System.ServiceModel.Channels.Binding binding) { }
        public BindingConfiguration(System.ServiceModel.Channels.Binding binding, System.Uri address) { }
        public System.Uri Address { get; }
        public System.ServiceModel.Channels.Binding Binding { get; }
    }
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://nservicebus.com")]
    public interface IWcfService<in TRequest, TResponse>
    
    
    {
        [System.ServiceModel.OperationContractAttribute()]
        System.Threading.Tasks.Task<TResponse> Process(TRequest request);
    }
    public class static WcfEndpointConfigurationExtensions
    {
        public static NServiceBus.WcfSettings Wcf(this NServiceBus.EndpointConfiguration configuration) { }
    }
    [System.ServiceModel.ServiceBehaviorAttribute(ConcurrencyMode=System.ServiceModel.ConcurrencyMode.Multiple, InstanceContextMode=System.ServiceModel.InstanceContextMode.PerCall)]
    public abstract class WcfService<TRequest, TResponse> : NServiceBus.IWcfService<TRequest, TResponse>
    
    
    {
        protected WcfService() { }
    }
    public class WcfSettings : NServiceBus.Configuration.AdvancedExtensibility.ExposeSettings
    {
        public NServiceBus.WcfSettings Binding(System.Func<System.Type, NServiceBus.BindingConfiguration> provider) { }
        public NServiceBus.WcfSettings CancelAfter(System.Func<System.Type, System.TimeSpan> provider) { }
        public NServiceBus.WcfSettings RouteWith(System.Func<System.Type, System.Func<NServiceBus.SendOptions>> provider) { }
    }
}