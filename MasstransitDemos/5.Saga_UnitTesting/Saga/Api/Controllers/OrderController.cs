using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Saga.Contracts;

namespace Saga.Api.Controllers;
[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
   
    private readonly ILogger<OrderController> _logger;
    private readonly IRequestClient<SubmitOrder> _requestClientSubmitOrder;
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly IRequestClient<CheckOrder> _checkOrderClient;
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderController(ILogger<OrderController> logger
        , IRequestClient<SubmitOrder> requestClientSubmitOrder
        , ISendEndpointProvider sendEndpointProvider
        ,IRequestClient<CheckOrder> checkOrderClient
        ,IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _requestClientSubmitOrder = requestClientSubmitOrder;
        _sendEndpointProvider = sendEndpointProvider;
        _checkOrderClient = checkOrderClient;
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost]
    public async Task<IActionResult> Post(Guid? id, string customerNumber)
    {
        if (id == null) 
        {
            id = Guid.NewGuid();
        }

        var (accepted, rejected) =  await _requestClientSubmitOrder.GetResponse<OrderSubmissionAccepted, OrderSubmissionRejected>(new
        {
            OrderId = id.Value,
            TimeStamp = InVar.Timestamp,
            CustomerNumber = customerNumber
        });

        if (accepted.IsCompletedSuccessfully)
        {
            return Ok(await accepted);
        }
        else 
        {
            return BadRequest(await rejected);
        }
        
    }

    [HttpPut]
    public async Task<IActionResult> Put(Guid? id, string customerNumber)
    {
        if (id == null)
        {
            id = Guid.NewGuid();
        }

        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("exchange:submit-order"));

        await endpoint.Send<SubmitOrder>( new {
            OrderId = id.Value,
            TimeStamp = InVar.Timestamp,
            CustomerNumber = customerNumber
        });

        return Accepted();

    }

    [HttpPatch]
    public async Task<IActionResult> Patch(Guid id)
    {
        await _publishEndpoint.Publish<OrderProcessingRequested>(new
        {
            OrderId = id
        });

        return Ok();

    }

    [HttpDelete]
    public async Task<IActionResult> Delete(Guid id)
    {
        
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("exchange:cancel-order"));

        await endpoint.Send<CancelOrder>(new
        {
            OrderId = id,
            TimeStamp = InVar.Timestamp
        });

        return Ok();

    }

    [HttpGet]
    public async Task<IActionResult> Get(Guid id)
    {

        var (status, notFound) = await _checkOrderClient.GetResponse<OrderStatus, OrderNotFound>(new
        {
            OrderId = id
        });

        if (status.IsCompletedSuccessfully)
        {
            return Ok((await status).Message);
        }
        else 
        {
            return NotFound((await notFound).Message);
        }
       
        

    }
}
