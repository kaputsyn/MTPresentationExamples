using MassTransit.Courier;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Courier.Components.Purchase
{
    public class PurchaseActivity : IActivity<PurchaseActivityArguments, PurchaseActivityLog>
    {
        private readonly ILogger<PurchaseActivity> _logger;


        public PurchaseActivity(ILogger<PurchaseActivity> logger)
        {
            _logger = logger;
        }
        public async Task<ExecutionResult> Execute(ExecuteContext<PurchaseActivityArguments> context)
        {
            _logger.LogInformation($"Getting: {context.Arguments.Sum} from: {context.Arguments.CustomerName}");
            await Task.Delay(3000);

            var tranId = Guid.NewGuid();
            _logger.LogInformation($"Successfully with transactionId: {tranId}");

            return context.Completed<PurchaseActivityLog>(new { TransactionId = tranId.ToString() });
        }

        public async Task<CompensationResult> Compensate(CompensateContext<PurchaseActivityLog> context)
        {
            _logger.LogInformation($"Reversig transaction: {context.Log.TransactionId}");

            return context.Compensated();
        }


    }
}
