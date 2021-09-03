using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using Microsoft.Extensions.Logging;
using Saga.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Saga.Components.Consumers
{
    public class SubmitOrderConsumer : IConsumer<SubmitOrder>
    {
        private readonly ILogger<SubmitOrderConsumer> _logger;

        public SubmitOrderConsumer(ILogger<SubmitOrderConsumer> logger)
        {
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<SubmitOrder> context)
        {
            _logger.LogDebug($"SubmitOrder message received for customer: {context.Message.CustomerNumber}");
            if (context.Message.CustomerNumber.Contains("TEST")) 
            {
                if (context.RequestId != null) 
                {
                    await context.RespondAsync<OrderSubmissionRejected>(new
                    {
                        OrderId = context.Message.OrderId,
                        TimeStamp = InVar.Timestamp,
                        CustomerNumber = context.Message.CustomerNumber
,
                        Reason = "Because of test"
                    });
                }
                
            }

            await context.Publish<OrderSubmitted>(new
            {
                OrderId = context.Message.OrderId,
                TimeStamp = InVar.Timestamp,
                CustomerNumber = context.Message.CustomerNumber
            });


            if (context.RequestId != null)
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
    public class SubmitOrderConsumerDefinition : ConsumerDefinition<SubmitOrderConsumer> 
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<SubmitOrderConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r => r.Interval(3, 1000));
        }
    }
}
