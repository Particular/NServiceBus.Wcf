namespace NServiceBus.AcceptanceTests
{
    using System;
    using System.ServiceModel;
    using System.Threading.Tasks;
    using AcceptanceTesting;
    using EndpointTemplates;
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

            Assert.IsTrue(context.HandlerCalled);
            Assert.AreEqual(Response.Ok, context.Response);
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
                    c.Wcf().Binding(t => new BindingConfiguration(new NetNamedPipeBinding(), new Uri("net.pipe://localhost/MyService")));
                });
            }

            public class MyService : WcfService<MyMessage, Response>
            {
            }

            public class MyMessageHandler : IHandleMessages<MyMessage>
            {
                public Context Context { get; set; }

                public Task Handle(MyMessage message, IMessageHandlerContext context)
                {
                    Context.HandlerCalled = true;
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
}