using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Worker
{
    internal class TransientErrorConsumer : IConsumer<ITransienrErrorCommand>
    {
        private readonly ILogger<TransientErrorConsumer> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public TransientErrorConsumer(ILogger<TransientErrorConsumer> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }
        public async Task Consume(ConsumeContext<ITransienrErrorCommand> context)
        {

            using (var client = _httpClientFactory.CreateClient()) 
            {
                client.BaseAddress = new Uri("http://localhost:5001");
                var response  = await client.GetAsync("faulty");

                if (!response.IsSuccessStatusCode) 
                {
                    _logger.LogInformation("Exception will be thown");
                    throw new ArgumentException("Transient error occured");
                }
            }

            _logger.LogInformation("ITransienrErrorCommand successfully handled");

            return;
        }
    }
}
