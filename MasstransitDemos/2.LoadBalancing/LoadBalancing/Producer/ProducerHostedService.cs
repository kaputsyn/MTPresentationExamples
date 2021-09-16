using Contracts;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Producer
{
    internal class ProducerHostedService : BackgroundService
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly ILogger<ProducerHostedService> _logger;

        public ProducerHostedService(ISendEndpointProvider sendEndpointProvider, ILogger<ProducerHostedService> logger)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("exchange:job"));

            for (int i = 0; i < 1000; i++) 
            {
                await sendEndpoint.Send<IDoJob>(new
                {
                    JobDescription = i.ToString()
                });
                _logger.LogInformation($"Event has been sent: {i}");

                await Task.Delay(100);
            }
        }
    }
}
