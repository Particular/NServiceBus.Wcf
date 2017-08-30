using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using ApprovalTests;
using NServiceBus;
using NUnit.Framework;
using PublicApiGenerator;

[TestFixture]
public class APIApprovals
{
    [Test]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public void Approve()
    {
        Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
        var publicApi = Filter(ApiGenerator.GeneratePublicApi(typeof(WcfSettings).Assembly));
        Approvals.Verify(publicApi);
    }

    string Filter(string text)
    {
        return string.Join(Environment.NewLine, text.Split(new[]
            {
                Environment.NewLine
            }, StringSplitOptions.RemoveEmptyEntries)
            .Where(l => !string.IsNullOrWhiteSpace(l))
        );
    }
}