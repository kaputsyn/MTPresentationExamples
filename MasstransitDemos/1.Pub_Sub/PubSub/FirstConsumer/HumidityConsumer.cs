using Contracts;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumidityListener
{
    internal class HumidityConsumer : IConsumer<IHumidityChanged>
    {
        public Task Consume(ConsumeContext<IHumidityChanged> context)
        {
            Console.WriteLine($"IHumidityChanged event consumed. New humidity: {context.Message.NewHumidityPercent} %");

            return Task.CompletedTask;
        }
    }
}
