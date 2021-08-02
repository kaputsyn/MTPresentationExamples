using Contracts;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace Components
{
    public class SubmitOrderConsumer : IConsumer<SubmitOrder>
    {
        public async Task Consume(ConsumeContext<SubmitOrder> context)
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
