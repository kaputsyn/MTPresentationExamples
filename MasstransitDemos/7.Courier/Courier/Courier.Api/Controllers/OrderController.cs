using Courier.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Courier.Api.Controllers;
[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
   
    private readonly ILogger<OrderController> _logger;
    private readonly IRequestClient<SubmitOrder> _requestClient;

    public OrderController(ILogger<OrderController> logger, IRequestClient<SubmitOrder> requestClient)
    {
        _logger = logger;
        _requestClient = requestClient;
    }

    [HttpPost]
    [Produces(typeof(OrderSubmitted))]
    public async Task<IActionResult> Post(SubmitOrderRequest submitOrderRequest)
    {
        var resp =  await _requestClient.GetResponse<OrderSubmitted>(new
        {
            Items = submitOrderRequest.Items
            .Select(x => new Courier.Contracts.OrderItem {Name = x.Name, Amount = x.Amount, Price = x.Price })
            .ToList(),
            CustomerName = submitOrderRequest.CustomerName,
            CustomerCard = submitOrderRequest.CustomerCard,
            Address = submitOrderRequest.Address
        });

        return Ok(resp);
    }
}
