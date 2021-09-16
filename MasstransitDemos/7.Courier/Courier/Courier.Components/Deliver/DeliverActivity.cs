using MassTransit.Courier;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Courier.Components.Deliver
{
    public class DeliverActivity : IExecuteActivity<DeliverActivityArguments>
    {
        private readonly ILogger<DeliverActivity> _logger;
        private static readonly List<string> _supportedAddresses = new List<string>() {"Kyiv","Lviv","Odesa","Kharkiv","Dnipro" };
        public DeliverActivity(ILogger<DeliverActivity> logger)
        {
            _logger = logger;
        }
        public async Task<ExecutionResult> Execute(ExecuteContext<DeliverActivityArguments> context)
        {
            _logger.LogInformation($"Trying to deliver an order: {context.Arguments.OrderId}");

            if (!_supportedAddresses.Contains(context.Arguments.Address)) 
            {
                _logger.LogWarning($"Address: {context.Arguments.Address} is not supported");
                throw new InvalidOperationException($"Address: {context.Arguments.Address} is not supported");
            }
            _logger.LogInformation($"Delivered successfully order: {context.Arguments.OrderId}");
            return context.Completed();
        }
    }
}
