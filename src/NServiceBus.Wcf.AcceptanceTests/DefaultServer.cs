﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.AcceptanceTesting.Customization;
using NServiceBus.AcceptanceTesting.Support;

public class DefaultServer : IEndpointSetupTemplate
{
    public DefaultServer()
    {
        typesToInclude = new List<Type>();
    }

    public DefaultServer(List<Type> typesToInclude)
    {
        this.typesToInclude = typesToInclude;
    }

    public async Task<EndpointConfiguration> GetConfiguration(RunDescriptor runDescriptor, EndpointCustomizationConfiguration endpointConfiguration, Func<EndpointConfiguration, Task> configurationBuilderCustomization)
    {
        var types = endpointConfiguration.GetTypesScopedByTestClass();

        typesToInclude.AddRange(types);

        var configuration = new EndpointConfiguration(endpointConfiguration.EndpointName);

        configuration.TypesToIncludeInScan(typesToInclude);
        configuration.EnableInstallers();

        var recoverability = configuration.Recoverability();
        recoverability.Delayed(delayed => delayed.NumberOfRetries(0));
        recoverability.Immediate(immediate => immediate.NumberOfRetries(0));

        var storageDir = Path.Combine(NServiceBusAcceptanceTest.StorageRootDir, NUnit.Framework.TestContext.CurrentContext.Test.ID);

        configuration.UseTransport(new LearningTransport { StorageDirectory = storageDir });

        configuration.RegisterComponentsAndInheritanceHierarchy(runDescriptor);

        await configurationBuilderCustomization(configuration);

        return configuration;
    }

    List<Type> typesToInclude;
}
