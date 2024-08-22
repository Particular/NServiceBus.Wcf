using System;
using System.ServiceModel;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.AcceptanceTesting;
using NUnit.Framework;

public class When_sending_to_a_different_endpoint : NServiceBusAcceptanceTest
{
    [Test]
    public async Task Response_should_be_received()
    {
        var messageId = Guid.NewGuid();

        var context = await Scenario.Define<Context>()
            .WithEndpoint<AnotherEndpoint>()
            .WithEndpoint<Endpoint>(b => b.When(async (session, c) =>
            {
                var pipeFactory = new ChannelFactory<IWcfService<MyMessage, MyResponse>>(new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/MyService"));
                var pipeProxy = pipeFactory.CreateChannel();
                var response = await pipeProxy.Process(new MyMessage
                {
                    Id = messageId
                });
                c.Id = response.Id;
            }))
            .Done(c => c.HandlerCalled && c.Id.HasValue)
            .Run();

        Assert.Multiple(() =>
        {
            Assert.That(context.HandlerCalled, Is.True);
            Assert.That(context.Id, Is.EqualTo(messageId));
        });
    }

    class Context : ScenarioContext
    {
        public bool HandlerCalled { get; set; }
        public Guid? Id { get; set; }
    }

    class Endpoint : EndpointConfigurationBuilder
    {
        public Endpoint()
        {
            EndpointSetup<DefaultServer>(c =>
            {
                c.MakeInstanceUniquelyAddressable("1");
                c.EnableCallbacks();
                c.Wcf()
                    .Binding(t => new BindingConfiguration(new NetNamedPipeBinding(), new Uri("net.pipe://localhost/MyService")))
                    .RouteWith(t => () =>
                    {
                        var options = new SendOptions();
                        options.SetDestination(NServiceBus.AcceptanceTesting.Customization.Conventions.EndpointNamingConvention(typeof(AnotherEndpoint)));
                        return options;
                    });
            });
        }

        public class MyService : WcfService<MyMessage, MyResponse>
        {
        }
    }

    class AnotherEndpoint : EndpointConfigurationBuilder
    {
        public AnotherEndpoint()
        {
            EndpointSetup<DefaultServer>(c =>
            {
                c.MakeInstanceUniquelyAddressable("1");
            });
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
