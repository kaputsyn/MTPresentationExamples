using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Components
{
    /// <summary>
    /// All Consumers are scoped
    /// </summary>
    public class SubmitOrderConsumer : IConsumer<SubmitOrder>
    {
        private readonly ILogger<SubmitOrderConsumer> _logger;

        public SubmitOrderConsumer(ILogger<SubmitOrderConsumer> logger)
        {
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<SubmitOrder> context)
        {
            _logger.LogDebug("SubmitOrderConsumer: {CustomerNumber}", context.Message.CustomerNumber);

            if (context.Message.CustomerNumber.Contains("TEST"))
            {
                if (context.ResponseAddress != null)
                {
                    await context.RespondAsync<OrderSubmissionRejected>(new
                    {
                        OrderId = context.Message.OrderId,
                        TimeStamp = InVar.Timestamp,
                        CustomerNumber = context.Message.CustomerNumber,
                        Reason = $"Test Customer cannot submit orders: {context.Message.CustomerNumber}"
                    });
                }
                return;
            }

            await  context.Publish<OrderSubmited>(new
            {
                OrderId = context.Message.OrderId,
                TimeStamp = InVar.Timestamp,
                CustomerNumber = context.Message.CustomerNumber
            });

            if (context.ResponseAddress != null)
            {
                await context.RespondAsync<OrderSubmissionAccepted>(new
                {
                    OrderId = context.Message.OrderId,
                    TimeStamp = InVar.Timestamp,
                    CustomerNumber = context.Message.CustomerNumber
                });
            }
        }
    }
}
