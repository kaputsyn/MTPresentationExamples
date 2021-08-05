﻿using Contracts;
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
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public OrderController(ILogger<OrderController> logger
            , IRequestClient<SubmitOrder> requestClient
            ,ISendEndpointProvider sendEndpointProvider)
        {
            _logger = logger;
            _requestClient = requestClient;
            _sendEndpointProvider = sendEndpointProvider;
        }
        [HttpPost]
        public async Task<IActionResult> Post(Guid id, string customerNumber) 
        {
            var (accepted, rejected) = await _requestClient.GetResponse<OrderSubmissionAccepted, OrderSubmissionRejected>(new
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
