using System;
using NServiceBus.AcceptanceTesting;
using NServiceBus.Logging;

public class StaticLoggerFactory : ILoggerFactory
{
    public StaticLoggerFactory(ScenarioContext ctx)
    {
        this.ctx = ctx;
    }

    public ILog GetLogger(Type type)
    {
        return new StaticLogger(ctx);
    }

    public ILog GetLogger(string name)
    {
        return new StaticLogger(ctx);
    }

    ScenarioContext ctx;
}