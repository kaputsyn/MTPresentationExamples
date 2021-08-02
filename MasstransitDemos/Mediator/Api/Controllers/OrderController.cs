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
        private readonly IRequestClient<SubmitOrder> _requestClient;

        public OrderController(ILogger<OrderController> logger
            , IRequestClient<SubmitOrder> requestClient)
        {
            _logger = logger;
            _requestClient = requestClient;
        }
        [HttpPost]
        public async Task<IActionResult> Post(Guid id, string customerNumber) 
        {
            Response<OrderSubmissionAccepted> res = await _requestClient.GetResponse<OrderSubmissionAccepted>(new
           {
               OrderId = id,
               TimeStamp = InVar.Timestamp,
               CustomerNumber = customerNumber
           });

          return Ok(res.Message);
        }

    }
}
