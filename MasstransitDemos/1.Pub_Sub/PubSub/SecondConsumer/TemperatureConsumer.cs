using Contracts;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemperatureListener
{
    internal class TemperatureConsumer : IConsumer<ITemperatureChanged>
    {
        public Task Consume(ConsumeContext<ITemperatureChanged> context)
        {
            Console.WriteLine($"IHumidityChanged event consumed. New temperature: {context.Message.NewTemperatureKelvin} K");

            return Task.CompletedTask;
        }
    }
}
