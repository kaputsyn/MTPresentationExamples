using MassTransit;
using Microsoft.Extensions.Logging;
using Saga.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Saga.Components.Consumers
{
    public class ProcessOrderConsumer : IConsumer<ProcessOrder>
    {
        private readonly ILogger<ProcessOrderConsumer> _logger;

        public ProcessOrderConsumer(ILogger<ProcessOrderConsumer> logger)
        {
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<ProcessOrder> context)
        {
            _logger.LogInformation($"Processing order: {context.Message.OrderId} for customer: {context.Message.CustomerNumber}");

            await Task.Delay(1000);

            _logger.LogInformation($"Order: {context.Message.OrderId} processed");

            await context.Publish<OrderProcessed>(new
            {
                OrderId = context.Message.OrderId,
                TimeStamp = InVar.Timestamp
            });
        }
    }
}
