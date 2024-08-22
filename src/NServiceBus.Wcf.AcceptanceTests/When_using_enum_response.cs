using System;
using System.ServiceModel;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.AcceptanceTesting;
using NUnit.Framework;

public class When_using_enum_response : NServiceBusAcceptanceTest
{
    public enum Response
    {
        Ok
    }

    [Test]
    public async Task Response_should_be_received()
    {
        var messageId = Guid.NewGuid();

        var context = await Scenario.Define<Context>()
            .WithEndpoint<Endpoint>(b => b.When(async (session, c) =>
            {
                var pipeFactory = new ChannelFactory<IWcfService<MyMessage, Response>>(new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/MyService"));
                var pipeProxy = pipeFactory.CreateChannel();
                var response = await pipeProxy.Process(new MyMessage
                {
                    Id = messageId
                });
                c.Response = response;
            }))
            .Done(c => c.HandlerCalled && c.Response.HasValue)
            .Run();

        Assert.Multiple(() =>
        {
            Assert.That(context.HandlerCalled, Is.True);
            Assert.That(context.Response, Is.EqualTo(Response.Ok));
        });
    }

    class Context : ScenarioContext
    {
        public bool HandlerCalled { get; set; }
        public Response? Response { get; set; }
    }

    class Endpoint : EndpointConfigurationBuilder
    {
        public Endpoint()
        {
            EndpointSetup<DefaultServer>(c =>
            {
                c.MakeInstanceUniquelyAddressable("1");
                c.EnableCallbacks();
                c.Wcf().Binding(t => new BindingConfiguration(new NetNamedPipeBinding(), new Uri("net.pipe://localhost/MyService")));
            });
        }

        public class MyService : WcfService<MyMessage, Response>
        {
        }

        public class MyMessageHandler : IHandleMessages<MyMessage>
        {
            Context testContext;

            public MyMessageHandler(Context testContext)
            {
                this.testContext = testContext;
            }

            public Task Handle(MyMessage message, IMessageHandlerContext context)
            {
                testContext.HandlerCalled = true;
                return context.Reply(Response.Ok);
            }
        }
    }

    [Serializable]
    public class MyMessage : ICommand
    {
        public Guid Id { get; set; }
    }
}