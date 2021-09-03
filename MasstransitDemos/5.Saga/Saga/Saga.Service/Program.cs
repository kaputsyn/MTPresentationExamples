
using MassTransit;
using MassTransit.Definition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Saga.Components.Consumers;
using Saga.Service;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Saga.Components;

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
                     x.AddConsumer<SubmitOrderConsumer>();

                     x.AddSagaStateMachine<OrderStateMachine, OrderState>(typeof(OrderStateMachineDefinition))
                     .RedisRepository();
                    
                     x.UsingRabbitMq((context, configurator) =>
                     {

                         configurator.ConfigureEndpoints(context);

                     });

                 });

                 services.AddHostedService<MassTransitHostedService>();

             })
             .ConfigureLogging((hostingContext, logging) => {

                 logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                 logging.AddConsole();
             });
        await builder.RunConsoleAsync();



