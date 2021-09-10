using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Saga.Components.Consumers;
using Saga.Contracts;

namespace Saga.Components.Tests;

[TestFixture]
public class When_an_order_request_is_consumed
{

    [Test]
    public async Task Should_respond_with_acceptance_if_ok()
    {
        var mockLogger = new Mock<ILogger<SubmitOrderConsumer>>();

        var harness = new InMemoryTestHarness();
        var consumer =  harness.Consumer<SubmitOrderConsumer>(() => new SubmitOrderConsumer(mockLogger.Object));
        



        await harness.Start();

        try
        {
            var orderId = Guid.NewGuid();

            var requestClient = await harness.ConnectRequestClient<SubmitOrder>();

            var response =  await requestClient.GetResponse<OrderSubmissionAccepted>(new
            {
                OrderId = orderId,
                TimeStamp = InVar.Timestamp,
                CustomerNumber = "12345"
            });

            Assert.That(response.Message.OrderId, Is.EqualTo(orderId));

            Assert.That(consumer.Consumed.Select<SubmitOrder>().Any(), Is.True);

            Assert.That(harness.Sent.Select<OrderSubmissionAccepted>().Any(), Is.True);
        }
        finally
        {
            await harness.Stop();
        }
    }


    [Test]
    public async Task Should_respond_with_rejected_if_test()
    {
        var mockLogger = new Mock<ILogger<SubmitOrderConsumer>>();

        var harness = new InMemoryTestHarness();
        var consumer = harness.Consumer<SubmitOrderConsumer>(() => new SubmitOrderConsumer(mockLogger.Object));




        await harness.Start();

        try
        {
            var orderId = Guid.NewGuid();

            var requestClient = await harness.ConnectRequestClient<SubmitOrder>();

            var response = await requestClient.GetResponse<OrderSubmissionRejected>(new
            {
                OrderId = orderId,
                TimeStamp = InVar.Timestamp,
                CustomerNumber = "12345TEST"
            });

            Assert.That(response.Message.OrderId, Is.EqualTo(orderId));

            Assert.That(consumer.Consumed.Select<SubmitOrder>().Any(), Is.True);

            Assert.That(harness.Sent.Select<OrderSubmissionRejected>().Any(), Is.True);
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Test]
    public async Task Shouldconsume_submit_order_command()
    {
        var mockLogger = new Mock<ILogger<SubmitOrderConsumer>>();

        var harness = new InMemoryTestHarness() { TestTimeout = TimeSpan.FromSeconds(1) };

        var consumer = harness.Consumer<SubmitOrderConsumer>(() => new SubmitOrderConsumer(mockLogger.Object));

        await harness.Start();

        try
        {
            var orderId = Guid.NewGuid();
            await harness.InputQueueSendEndpoint.Send<SubmitOrder>(new
            {
                OrderId = orderId,
                TimeStamp = InVar.Timestamp,
                CustomerNumber = "12345"
            });

            Assert.That(consumer.Consumed.Select<SubmitOrder>().Any(), Is.True);
            Assert.That(harness.Sent.Select<OrderSubmissionRejected>().Any(), Is.False);
            Assert.That(harness.Sent.Select<OrderSubmissionAccepted>().Any(), Is.False);
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Test]
    public async Task Should_publish_order_submited_event()
    {
        var mockLogger = new Mock<ILogger<SubmitOrderConsumer>>();

        var harness = new InMemoryTestHarness() { TestTimeout = TimeSpan.FromSeconds(1) };

        var consumer = harness.Consumer<SubmitOrderConsumer>(() => new SubmitOrderConsumer(mockLogger.Object));

        await harness.Start();

        try
        {
            var orderId = Guid.NewGuid();
            await harness.InputQueueSendEndpoint.Send<SubmitOrder>(new
            {
                OrderId = orderId,
                TimeStamp = InVar.Timestamp,
                CustomerNumber = "12345"
            });

            Assert.That(consumer.Consumed.Select<SubmitOrder>().Any(), Is.True);
            Assert.That(harness.Published.Select<OrderSubmitted>().Any(), Is.True);
        }
        finally
        {
            await harness.Stop();
        }
    }


    [Test]
    public async Task Should_not_publish_order_submited_event_when_rejected()
    {
        var mockLogger = new Mock<ILogger<SubmitOrderConsumer>>();

        var harness = new InMemoryTestHarness() { TestTimeout = TimeSpan.FromSeconds(1) };

        var consumer = harness.Consumer<SubmitOrderConsumer>(() => new SubmitOrderConsumer(mockLogger.Object));

        await harness.Start();

        try
        {
            var orderId = Guid.NewGuid();
            await harness.InputQueueSendEndpoint.Send<SubmitOrder>(new
            {
                OrderId = orderId,
                TimeStamp = InVar.Timestamp,
                CustomerNumber = "TEST"
            });

            Assert.That(consumer.Consumed.Select<SubmitOrder>().Any(), Is.True);
            Assert.That(harness.Published.Select<OrderSubmitted>().Any(), Is.False);
        }
        finally
        {
            await harness.Stop();
        }
    }

}