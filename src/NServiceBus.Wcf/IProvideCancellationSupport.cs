namespace NServiceBus
{
    using System;

    interface IProvideCancellationSupport
    {
        TimeSpan CancelAfter { get; set; }
    }
}