using MassTransit.Courier;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Courier.Components.Authorize
{
    public class AuthorizeActivity : IExecuteActivity<AuthorizeActivityArguments>
    {
        private readonly ILogger<AuthorizeActivity> _logger;

        public AuthorizeActivity(ILogger<AuthorizeActivity> logger)
        {
            _logger = logger;
        }
        public async Task<ExecutionResult> Execute(ExecuteContext<AuthorizeActivityArguments> context)
        {
            _logger.LogInformation($"Authorizing customer: {context.Arguments.CustomerName} with card: {context.Arguments.CustomerCard}");

            await Task.Delay(2000);

            if (context.Arguments.CustomerName.Contains("TEST", StringComparison.InvariantCultureIgnoreCase))
            {
                return context.Faulted();
            }
            
            
            return context.Completed();
        }
    }
}
