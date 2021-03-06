using System;
using System.IO;
using System.Linq;
using System.Threading;
using NServiceBus.AcceptanceTesting;
using NServiceBus.AcceptanceTesting.Customization;
using NUnit.Framework;

[TestFixture]
public abstract class NServiceBusAcceptanceTest
{
    [SetUp]
    public void SetUp()
    {
        // Hack: prevents SerializationException ... Type 'x' in assembly 'y' is not marked as serializable.
        // https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/mitigation-deserialization-of-objects-across-app-domains
        System.Configuration.ConfigurationManager.GetSection("X");

        Conventions.EndpointNamingConvention = t =>
        {
            var classAndEndpoint = t.FullName.Split('.').Last();

            var testName = classAndEndpoint.Split('+').First();

            testName = testName.Replace("When_", "");

            var endpointBuilder = classAndEndpoint.Split('+').Last();


            testName = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(testName);

            testName = testName.Replace("_", "");

            return testName + "." + endpointBuilder;
        };
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Scenario.GetLoggerFactory = ctx => new StaticLoggerFactory(ctx);

        if (Directory.Exists(StorageRootDir))
        {
            Directory.Delete(StorageRootDir, true);
        }
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        if (Directory.Exists(StorageRootDir))
        {
            Directory.Delete(StorageRootDir, true);
        }
    }

    public static string StorageRootDir
    {
        get
        {
            string tempDir;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                //can't use bin dir since that will be too long on the build agents
                tempDir = @"c:\temp";
            }
            else
            {
                tempDir = Path.GetTempPath();
            }

            return Path.Combine(tempDir, "callback-acpttests");
        }
    }
}