using Components;
using Components.StateMachines;
using MassTransit;
using MassTransit.Definition;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Service
{
    class Program
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
                     services.Configure<AppConfig>(hostContext.Configuration.GetSection("AppConfig"));
                     services.AddSingleton(KebabCaseEndpointNameFormatter.Instance);
                     services.AddMassTransit(x =>
                     {
                         x.AddConsumer<SubmitOrderConsumer>();
                         x.AddSagaStateMachine<OrderStateMachine, OrderState>(typeof(OrderStateMachineDefinition))
                         .InMemoryRepository();

                         x.UsingRabbitMq(ConfigureBus);

                     });

                     services.AddHostedService<MassTransitHostedService>();

                 })
                 .ConfigureLogging((hostingContext, logging) => {

                     logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                     logging.AddConsole();
                 });
            await builder.RunConsoleAsync();

        }

        static void ConfigureBus(IBusRegistrationContext context, IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.ConfigureEndpoints(context);
            var config =  context.GetService<IOptions<AppConfig>>().Value;

            configurator.Host(config.RabbitMq.HostAddresses, config.RabbitMq.VirtualHost, h => {
                h.Username(config.RabbitMq.UserName);
                h.Password(config.RabbitMq.Password);
            });
        }
    }
}
