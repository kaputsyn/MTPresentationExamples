using MassTransit;
using Microsoft.Extensions.Logging;
using Saga.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Saga.Components.Consumers
{
    public class CancelOrderConsumer : IConsumer<CancelOrder>
    {
        private readonly ILogger<CancelOrderConsumer> _logger;

        public CancelOrderConsumer(ILogger<CancelOrderConsumer> logger)
        {
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<CancelOrder> context)
        {
            _logger.LogInformation($"Order: {context.Message.OrderId} cancelation requested at {context.Message.TimeStamp}");

            _logger.LogInformation($"Canceling order: {context.Message.OrderId}");
            await Task.Delay(1000);

            _logger.LogInformation($"Order {context.Message.OrderId} canceled");

            await context.Publish<OrderCanceled>(new
            {
                OrderId = context.Message.OrderId,
                TimeStamp = InVar.Timestamp
            });
            

        }
    }
}
