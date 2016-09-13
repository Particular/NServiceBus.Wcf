namespace NServiceBus.AcceptanceTests
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Threading.Tasks;
    using AcceptanceTesting;
    using EndpointTemplates;
    using NUnit.Framework;

    public class When_using_enum : NServiceBusAcceptanceTest
    {
        [Test]
        public async Task Message_should_be_received()
        {
            var context = await Scenario.Define<Context>()
                .WithEndpoint<Endpoint>(b => b.When((session, c) =>
                {
                    var pipeFactory = new ChannelFactory<IWcfService<MyMessage, MyResponse>>(new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/MyService"));
                    var pipeProxy = pipeFactory.CreateChannel();
                    return pipeProxy.Process(new MyMessage());
                }))
                .Done(c => c.HandlerCalled)
                .Run();

            Assert.IsTrue(context.HandlerCalled);
        }

        class Context : ScenarioContext
        {
            public bool HandlerCalled { get; set; }
        }

        class Endpoint : EndpointConfigurationBuilder
        {
            public Endpoint()
            {
                EndpointSetup<DefaultServer>(c =>
                {
                    c.MakeInstanceUniquelyAddressable("1");
                    c.BindingProvider(t => Tuple.Create<Binding, string>(new NetNamedPipeBinding(), "net.pipe://localhost/MyService"));
                });
            }

            public class MyService : WcfService<MyMessage, MyResponse> { }

            public class MyMessageHandler : IHandleMessages<MyMessage>
            {
                public Context Context { get; set; }

                public Task Handle(MyMessage message, IMessageHandlerContext context)
                {
                    Context.HandlerCalled = true;
                    return context.Reply(new MyResponse());
                }
            }
        }

        [Serializable]
        public class MyMessage : ICommand
        {
        }

        [Serializable]
        public class MyResponse : IMessage
        {
        }
    }
}