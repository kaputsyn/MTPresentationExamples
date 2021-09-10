using MassTransit;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saga.Service
{
    public class MassTransitHostedService : IHostedService
    {
        private readonly IBusControl _busControl;

        public MassTransitHostedService(IBusControl busControl)
        {
            _busControl = busControl;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _busControl.StartAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _busControl.StartAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
