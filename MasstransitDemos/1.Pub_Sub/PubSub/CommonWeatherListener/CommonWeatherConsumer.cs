using Contracts;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonWeatherListener
{
    internal class CommonWeatherConsumer : IConsumer<IWeatherChanged>
    {
        public Task Consume(ConsumeContext<IWeatherChanged> context)
        {
            Console.WriteLine("IWeatherChanged event consumed");

            return Task.CompletedTask;
        }
    }
}
