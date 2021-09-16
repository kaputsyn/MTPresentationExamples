using Courier.Contracts;
using MassTransit;
using MassTransit.Courier;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Courier.Components
{
    public class CourierSubmitOrderConsumer : IConsumer<SubmitOrder>
    {
        private readonly ILogger<CourierSubmitOrderConsumer> _logger;

        public CourierSubmitOrderConsumer(ILogger<CourierSubmitOrderConsumer> logger)
        {
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<SubmitOrder> context)
        {
            var builder = new RoutingSlipBuilder(Guid.NewGuid());

            var orderId = Guid.NewGuid();

            builder.AddVariable("OrderId", orderId);

            builder.AddActivity("Authorize", new Uri("exchange:authorize_execute"), new
            {
                CustomerName = context.Message.CustomerName,
                CustomerCard = context.Message.CustomerCard

            });

            builder.AddActivity("Purchase", new Uri("exchange:purchase_execute"), new
            {
                CustomerName = context.Message.CustomerName,
                CustomerCard = context.Message.CustomerCard,
                Sum = context.Message.Items.Sum(x => x.Price * x.Amount)

            });

            builder.AddActivity("Deliver", new Uri("exchange:deliver_execute"), new
            {
                Address = context.Message.Address,
                Items = context.Message.Items

            });

            var routingSlip = builder.Build();

            await context.Execute(routingSlip);

            await context.RespondAsync<OrderSubmitted>(new
            {
                OrderId = orderId
            });
        }
    }
}
