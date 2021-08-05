using Components.StateMachines;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Mediator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController: Controller
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IRequestClient<SubmitOrder> _requestClientSubmitOrder;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IRequestClient<CheckOrder> _requestClientCheckOrder;

        public OrderController(ILogger<OrderController> logger
            ,IRequestClient<SubmitOrder> requestClientSubmitOrder
            ,ISendEndpointProvider sendEndpointProvider
            ,IRequestClient<CheckOrder> requestClientCheckOrder)
        {
            _logger = logger;
            _requestClientSubmitOrder = requestClientSubmitOrder;
            _sendEndpointProvider = sendEndpointProvider;
            _requestClientCheckOrder = requestClientCheckOrder;
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid id) 
        {
            var (status, notFound) = await _requestClientCheckOrder.GetResponse<OrderStatus,OrderNotFound>(new
            {
                OrderId = id
            });

            if (status.IsCompletedSuccessfully)
            {
                var resp = await status;
                return Ok(resp.Message);
            }
            else 
            {
                var resp = await notFound;
                return NotFound(resp.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Post(Guid id, string customerNumber) 
        {
            var (accepted, rejected) = await _requestClientSubmitOrder.GetResponse<OrderSubmissionAccepted, OrderSubmissionRejected>(new
            {
                OrderId = id,
                TimeStamp = InVar.Timestamp,
                CustomerNumber = customerNumber
            });
            if (accepted.IsCompletedSuccessfully)
            {
                var response = await accepted;
                return Accepted(response);
            }
            else 
            {
                var response = await rejected;
                return BadRequest(response);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put(Guid id, string customerNumber)
        {
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"exchange:submit-order"));

            await endpoint.Send<SubmitOrder>(new
            {
                OrderId = id,
                TimeStamp = InVar.Timestamp,
                CustomerNumber = customerNumber
            });

            return Accepted();
        }

    }
}
