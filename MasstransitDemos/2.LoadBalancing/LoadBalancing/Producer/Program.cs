﻿using MassTransit;
using MassTransit.Definition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

namespace Producer 
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {

                    config.AddJsonFile("appsettings.json", true);
                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                 .ConfigureServices((hostContext, services) =>
                 {
                     services.AddSingleton(KebabCaseEndpointNameFormatter.Instance);
                     services.AddMassTransit(x =>
                     {
                         x.UsingRabbitMq((context, configurator) => {

                             configurator.ConfigureEndpoints(context);

                         });

                     });
                     services.AddHostedService<ProducerHostedService>();
                     services.AddHostedService<MassTransitHostedService>();

                 })
                 .ConfigureLogging((hostingContext, logging) => {

                     logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                     logging.AddConsole();
                 });

            var host =  builder.Build();
            await host.RunAsync();







        }


    }
}