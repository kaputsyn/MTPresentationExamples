using Components.StateMachines;
using Contracts;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Tests
{
    [TestFixture]
    public class SubmittingAnOrder
    {

        [Test]
        public async Task ShouldCreateAStateInstance()
        {
            var harness = new InMemoryTestHarness();
            harness.TestTimeout = TimeSpan.FromSeconds(1);

            var mock = new Mock<ILogger<SubmitOrderConsumer>>();
            var consumer = harness.StateMachineSaga<OrderState, OrderStateMachine>( new OrderStateMachine());

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
    }
}
