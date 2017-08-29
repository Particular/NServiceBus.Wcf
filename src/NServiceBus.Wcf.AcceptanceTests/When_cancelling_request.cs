using System;
using System.ServiceModel;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.AcceptanceTesting;
using NUnit.Framework;

public class When_cancelling_request : NServiceBusAcceptanceTest
{
    [Test]
    public async Task Response_should_not_be_received()
    {
        var messageId = Guid.NewGuid();

        var context = await Scenario.Define<Context>()
            .WithEndpoint<Endpoint>(b => b.When(async (session, c) =>
            {
                var pipeFactory = new ChannelFactory<IWcfService<MyMessage, MyResponse>>(new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/MyService"));
                var pipeProxy = pipeFactory.CreateChannel();
                try
                {
                    var response = await pipeProxy.Process(new MyMessage
                    {
                        Id = messageId
                    });
                    c.Id = response.Id;
                }
                catch (FaultException ex)
                {
                    c.Exception = ex;
                }
            }))
            .Done(c => c.Exception != null)
            .Run(TimeSpan.FromSeconds(10));

        Assert.That(context.Exception.Message, Does.Contain("The request was cancelled after"));
        Assert.IsFalse(context.HandlerCalled);
        Assert.IsNull(context.Id);
    }

    class Context : ScenarioContext
    {
        public bool HandlerCalled { get; set; }
        public FaultException Exception { get; set; }
        public Guid? Id { get; set; }
    }

    class Endpoint : EndpointConfigurationBuilder
    {
        public Endpoint()
        {
            EndpointSetup<DefaultServer>(c =>
            {
                c.MakeInstanceUniquelyAddressable("1");
                c.Wcf()
                    .Binding(t => new BindingConfiguration(new NetNamedPipeBinding(), new Uri("net.pipe://localhost/MyService")))
                    .CancelAfter(t => t.IsAssignableFrom(typeof(MyService)) ? TimeSpan.Zero : TimeSpan.FromSeconds(5));
            });
        }

        public class MyService : WcfService<MyMessage, MyResponse>
        {
        }

        public class MyMessageHandler : IHandleMessages<MyMessage>
        {
            public Context Context { get; set; }

            public Task Handle(MyMessage message, IMessageHandlerContext context)
            {
                Context.HandlerCalled = true;
                return context.Reply(new MyResponse
                {
                    Id = message.Id
                });
            }
        }
    }

    public class MyMessage : ICommand
    {
        public Guid Id { get; set; }
    }

    public class MyResponse : IMessage
    {
        public Guid Id { get; set; }
    }
}
