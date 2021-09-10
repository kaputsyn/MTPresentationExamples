using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class WeatherConsumer : IConsumer<IGetWeather>
    {
        private readonly ILogger<WeatherConsumer> _logger;

        public WeatherConsumer(ILogger<WeatherConsumer> logger)
        {
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<IGetWeather> context)
        {
            _logger.LogInformation("IGetWeather message received");

            if (context.Message.Location == "Kyiv") 
            {
                await context.RespondAsync<IWeatherNotAwailable>(new
                {
                    Reason = "Because"
                });
            }


            var rnd = new Random();

            await context.RespondAsync<IWeatherResponse>(new
            {
                HumidityPercent = rnd.Next(20,100),
                TemperatureKelvin = rnd.Next(240, 330),
                TimeStamp = InVar.Timestamp,
                Location = context.Message.Location
            });
        }
    }
}
