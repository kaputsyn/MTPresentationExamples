using Contracts;
using MassTransit;
using Microsoft.Extensions.Hosting;
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

        public ProducerHostedService(ISendEndpointProvider sendEndpointProvider)
        {
            _sendEndpointProvider = sendEndpointProvider;
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
                await Task.Delay(100);
            }
        }
    }
}
