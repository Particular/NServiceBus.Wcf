namespace NServiceBus.AcceptanceTests
{
    using System;
    using System.ServiceModel;
    using System.Threading.Tasks;
    using AcceptanceTesting;
    using AcceptanceTesting.Customization;
    using EndpointTemplates;
    using NUnit.Framework;

    public class When_unobtrusive_sending_to_a_different_endpoint : NServiceBusAcceptanceTest
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

            Assert.IsTrue(context.HandlerCalled);
            Assert.AreEqual(messageId, context.Id);
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
                    c.Conventions()
                        .DefiningCommandsAs(t => t.Namespace != null && t.Name.EndsWith("MyMessage"))
                        .DefiningMessagesAs(t => t.Namespace != null && t.Name.EndsWith("MyResponse"));
                    c.Wcf()
                        .Binding(t => new BindingConfiguration(new NetNamedPipeBinding(), new Uri("net.pipe://localhost/MyService")))
                        .RouteWith(t => () =>
                        {
                            var options = new SendOptions();
                            options.SetDestination(Conventions.EndpointNamingConvention(typeof(AnotherEndpoint)));
                            return options;
                        });
                }).ExcludeType<MyMessage>();
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
                    c.Conventions()
                        .DefiningCommandsAs(t => t.Namespace != null && t.Name.EndsWith("MyMessage"))
                        .DefiningMessagesAs(t => t.Namespace != null && t.Name.EndsWith("MyResponse"));
                });
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

        public class MyMessage
        {
            public Guid Id { get; set; }
        }

        public class MyResponse
        {
            public Guid Id { get; set; }
        }
    }
}