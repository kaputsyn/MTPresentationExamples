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
    internal class FaultConsumer : IConsumer<Fault<ITransienrErrorCommand>>
    {
        private readonly ILogger<FaultConsumer> _logger;

        public FaultConsumer(ILogger<FaultConsumer> logger )
        {
            _logger = logger;
        }
        public Task Consume(ConsumeContext<Fault<ITransienrErrorCommand>> context)
        {
            _logger.LogWarning($"Error logged error: {context.Message.Message.ErrorCode}");

            return Task.CompletedTask;
        }
    }
}
