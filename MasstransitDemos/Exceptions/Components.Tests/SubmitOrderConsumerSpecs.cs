using Contracts;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Components.Tests
{
    [TestFixture]
    public class WhenOrderRequestConsumed
    {

        [Test]
        public async Task ShouldRespondWithAcceptenceIfOk() 
        {
            var harness = new InMemoryTestHarness();
            var mock = new Mock<ILogger<SubmitOrderConsumer>>();
            var consumer = harness.Consumer<SubmitOrderConsumer>(() => new SubmitOrderConsumer(mock.Object));

            await harness.Start();

            try
            {
                var orderId = NewId.NextGuid();

                var reqClient = await harness.ConnectRequestClient<SubmitOrder>();

                var response = await reqClient.GetResponse<OrderSubmissionAccepted>(new
                {
                    OrderId = orderId,
                    TimeStamp = InVar.Timestamp,
                    CustomerNumber = "123"
                });

                Assert.That(response.Message.OrderId, Is.EqualTo(orderId));
                Assert.That(harness.Sent.Select<OrderSubmissionAccepted>().Any(), Is.True);
                Assert.That(consumer.Consumed.Select<SubmitOrder>().Any(), Is.True);

            }
            finally
            {
                await harness.Stop();
            }


        }

        [Test]
        public async Task ShouldConsumeSubmitOrderCommands()
        {
            var harness = new InMemoryTestHarness();
            harness.TestTimeout = TimeSpan.FromSeconds(1);

            var mock = new Mock<ILogger<SubmitOrderConsumer>>();
            var consumer = harness.Consumer<SubmitOrderConsumer>(() => new SubmitOrderConsumer(mock.Object));

            await harness.Start();

            try
            {
                var newId = NewId.NextGuid();

                await harness.InputQueueSendEndpoint.Send<SubmitOrder>(new
                {
                    OrderId = newId,
                    TimeStamp = InVar.Timestamp,
                    CustomerNumber = "1234"
                });

                Assert.That(consumer.Consumed.Select<SubmitOrder>().Any(), Is.True);
                Assert.That(harness.Sent.Select<OrderSubmissionAccepted>().Any(), Is.False);
                Assert.That(harness.Sent.Select<OrderSubmissionRejected>().Any(), Is.False);

            }
            finally
            {
                await harness.Stop();
            }


        }

        [Test]
        public async Task ShouldRespondWithRejectedIfTest()
        {
            var harness = new InMemoryTestHarness();
            var mock = new Mock<ILogger<SubmitOrderConsumer>>();
            var consumer = harness.Consumer<SubmitOrderConsumer>(() => new SubmitOrderConsumer(mock.Object));

            await harness.Start();

            try
            {
                var orderId = NewId.NextGuid();

                var reqClient = await harness.ConnectRequestClient<SubmitOrder>();

                var response = await reqClient.GetResponse<OrderSubmissionRejected>(new
                {
                    OrderId = orderId,
                    TimeStamp = InVar.Timestamp,
                    CustomerNumber = "TEST123"
                });

                Assert.That(harness.Sent.Select<OrderSubmissionRejected>().Any(), Is.True);

            }
            finally
            {
                await harness.Stop();
            }


        }

        [Test]
        public async Task ShouldPublishOrderSubmitedEvent()
        {

            var harness = new InMemoryTestHarness();

            var mock = new Mock<ILogger<SubmitOrderConsumer>>();
            var consumer = harness.Consumer<SubmitOrderConsumer>(() => new SubmitOrderConsumer(mock.Object));

            await harness.Start();

            try
            {
                var newId = NewId.NextGuid();

                await harness.InputQueueSendEndpoint.Send<SubmitOrder>(new
                {
                    OrderId = newId,
                    TimeStamp = InVar.Timestamp,
                    CustomerNumber = "1234"
                });

                Assert.That(consumer.Consumed.Select<SubmitOrder>().Any(), Is.True);
                Assert.That(harness.Published.Select<OrderSubmited>().Any(), Is.True);
            }
            finally
            {
                await harness.Stop();
            }


        }

        [Test]
        public async Task ShouldNOTPublishOrderSubmitedEventWhenRejected()
        {

            var harness = new InMemoryTestHarness();
            harness.TestTimeout = TimeSpan.FromSeconds(1);

            var mock = new Mock<ILogger<SubmitOrderConsumer>>();
            var consumer = harness.Consumer<SubmitOrderConsumer>(() => new SubmitOrderConsumer(mock.Object));

            await harness.Start();

            try
            {
                var newId = NewId.NextGuid();

                await harness.InputQueueSendEndpoint.Send<SubmitOrder>(new
                {
                    OrderId = newId,
                    TimeStamp = InVar.Timestamp,
                    CustomerNumber = "TEST1234"
                });

                Assert.That(consumer.Consumed.Select<SubmitOrder>().Any(), Is.True);

                Assert.That(harness.Published.Select<OrderSubmited>().Any(), Is.False);

            }
            finally
            {
                await harness.Stop();
            }


        }

    }
}