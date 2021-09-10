using Automatonymous.Graphing;
using Automatonymous.Visualizer;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Saga.Components.Consumers;
using Saga.Contracts;

namespace Saga.Components.Tests
{
    [TestFixture]
    public class Submitting_an_order
    {

        [Test]
        public async Task Should_create_a_state_instance() 
        {
            var mockLogger = new Mock<ILogger<SubmitOrderConsumer>>();
            var orderStateMachine = new OrderStateMachine();

            var harness = new InMemoryTestHarness();
            var saga = harness.StateMachineSaga<OrderState,OrderStateMachine>(orderStateMachine);

            await harness.Start();

            try
            {
                var orderId = Guid.NewGuid();

                await harness.Bus.Publish<OrderSubmitted>(new
                {
                    OrderId = orderId,
                    TimeStamp = InVar.Timestamp,
                    CustomerNumber = "12345"
                });

                Assert.That(saga.Created.Select(x => x.CorrelationId == orderId).Any(), Is.True); 
                var instanceId = await saga.Exists(orderId, x => x.Submitted);

                Assert.That(instanceId, Is.Not.Null);

                var instance = saga.Sagas.Contains(instanceId.Value);
                Assert.That(instance.CustomerNumber, Is.EqualTo("12345"));
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Test]
        public async Task Should_respond_to_status_ckeks()
        {
            var mockLogger = new Mock<ILogger<SubmitOrderConsumer>>();
            var orderStateMachine = new OrderStateMachine();

            var harness = new InMemoryTestHarness();
            var saga = harness.StateMachineSaga<OrderState, OrderStateMachine>(orderStateMachine);

            await harness.Start();

            try
            {
                var orderId = Guid.NewGuid();

                await harness.Bus.Publish<OrderSubmitted>(new
                {
                    OrderId = orderId,
                    TimeStamp = InVar.Timestamp,
                    CustomerNumber = "12345"
                });

                Assert.That(saga.Created.Select(x => x.CorrelationId == orderId).Any(), Is.True);
                var instanceId = await saga.Exists(orderId, x => x.Submitted);

                Assert.That(instanceId, Is.Not.Null);

                var instance = saga.Sagas.Contains(instanceId.Value);
                Assert.That(instance.CustomerNumber, Is.EqualTo("12345"));

                var requestClient = await harness.ConnectRequestClient<CheckOrder>();

                var response = await requestClient.GetResponse<OrderStatus>(new
                {
                    OrderId = orderId
                });
                Assert.That(response.Message.State, Is.EqualTo(orderStateMachine.Submitted.Name));
            }
            finally
            {
                await harness.Stop();
            }
        }


        [Test]
        public async Task Should_cancel_when_customer_account_closed()
        {
            var mockLogger = new Mock<ILogger<SubmitOrderConsumer>>();
            var orderStateMachine = new OrderStateMachine();

            var harness = new InMemoryTestHarness();
            var saga = harness.StateMachineSaga<OrderState, OrderStateMachine>(orderStateMachine);

            await harness.Start();

            try
            {
                var orderId = Guid.NewGuid();

                await harness.Bus.Publish<OrderSubmitted>(new
                {
                    OrderId = orderId,
                    TimeStamp = InVar.Timestamp,
                    CustomerNumber = "12345"
                });

                Assert.That(saga.Created.Select(x => x.CorrelationId == orderId).Any(), Is.True);

                var instanceId = await saga.Exists(orderId, x => x.Submitted);
                Assert.That(instanceId, Is.Not.Null);

                await harness.Bus.Publish<CustomerAccountClosed>(new
                {
                    CustomerId = InVar.Id,
                    CustomerNumber = "12345"
                });

                instanceId = await saga.Exists(orderId, x => x.Canceled);
                Assert.That(instanceId, Is.Not.Null);


            }
            finally
            {
                await harness.Stop();
            }
        }

        [Test]
        public async Task Should_accept_when_order_is_accepted()
        {
            var mockLogger = new Mock<ILogger<SubmitOrderConsumer>>();
            var orderStateMachine = new OrderStateMachine();

            var harness = new InMemoryTestHarness();
            var saga = harness.StateMachineSaga<OrderState, OrderStateMachine>(orderStateMachine);

            await harness.Start();

            try
            {
                var orderId = Guid.NewGuid();

                await harness.Bus.Publish<OrderSubmitted>(new
                {
                    OrderId = orderId,
                    TimeStamp = InVar.Timestamp,
                    CustomerNumber = "12345"
                });

                Assert.That(saga.Created.Select(x => x.CorrelationId == orderId).Any(), Is.True);

                var instanceId = await saga.Exists(orderId, x => x.Submitted);
                Assert.That(instanceId, Is.Not.Null);

                await harness.Bus.Publish<OrderAccepted>(new
                {

                    OrderId = orderId,
                    TimeStamp = InVar.Timestamp
                });

                instanceId = await saga.Exists(orderId, x => x.Accepted);
                Assert.That(instanceId, Is.Not.Null);


            }
            finally
            {
                await harness.Stop();
            }
        }
    }
}
