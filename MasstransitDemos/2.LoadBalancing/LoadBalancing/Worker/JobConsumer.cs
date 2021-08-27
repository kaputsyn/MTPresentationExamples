using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worker
{
    internal class JobConsumer : IConsumer<IDoJob>
    {
        private readonly ILogger<JobConsumer> _logger;

        public JobConsumer(ILogger<JobConsumer> logger)
        {
            _logger = logger;
        }
        public Task Consume(ConsumeContext<IDoJob> context)
        {
            _logger.LogInformation($"Job Done: {context.Message.JobDescription}");
            return Task.CompletedTask;
        }
    }
}
