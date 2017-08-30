using System;
using NServiceBus.AcceptanceTesting;
using NServiceBus.Logging;

public class StaticLoggerFactory : ILoggerFactory
{
    public StaticLoggerFactory(ScenarioContext ctx)
    {
        CurrentContext = ctx;
    }

    public ILog GetLogger(Type type)
    {
        return new StaticLogger();
    }

    public ILog GetLogger(string name)
    {
        return new StaticLogger();
    }

    internal static ScenarioContext CurrentContext;
}