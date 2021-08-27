using Contracts;
using MassTransit;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Producer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Producer");

            var busControl = Bus.Factory.CreateUsingRabbitMq();

            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            await busControl.StartAsync(source.Token);
            try
            {
                Random r = new Random();
                while (true)
                {
                    string value = await Task.Run(() =>
                    {
                        Console.WriteLine("Chose event type: temperature(t), humidity(h) (or q to exit)");
                        Console.Write("> ");
                        return Console.ReadLine();
                    });

                    if ("q".Equals(value, StringComparison.OrdinalIgnoreCase))
                        break;
                    if ("t".Equals(value, StringComparison.OrdinalIgnoreCase)) 
                    {
                        var newTemp = r.Next(0, 330);
                        await busControl.Publish<ITemperatureChanged>(new
                        {
                            NewTemperatureKelvin = newTemp
                        });
                        Console.WriteLine($"ITemperatureChanged event sent. New temperature: {newTemp} K");
                        continue;
                    }
                    if ("h".Equals(value, StringComparison.OrdinalIgnoreCase))
                    {
                        var newHum = r.Next(10, 100);
                        await busControl.Publish<IHumidityChanged>(new
                        {
                            NewHumidityPercent = newHum
                        });
                        Console.WriteLine($"IHumidityChanged event sent. New humidity: {newHum} %");
                        continue;
                    }

                }
            }
            finally
            {
                await busControl.StopAsync();
            }
        }
    }
}
