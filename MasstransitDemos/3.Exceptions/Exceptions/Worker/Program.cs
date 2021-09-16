using GreenPipes;
using MassTransit;
using MassTransit.Definition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Worker;

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
                         x.AddConsumer<TransientErrorConsumer>();
                         x.AddConsumer<FaultConsumer>();

                         x.UsingRabbitMq((context, configurator) =>
                         {

                         configurator.ReceiveEndpoint("EXCEPTIONS-transient-error", e =>
                         {
                             e.ConfigureConsumer<TransientErrorConsumer>(context);
                             //e.UseMessageRetry(cfg =>
                             //{
                             //    cfg.Handle<ArgumentException>();



                             //    cfg.Interval(5, 1000);
                             //    });
                             e.UseKillSwitch(cfg =>
                             {
                                 cfg.ActivationThreshold = 5;
                                 cfg.RestartTimeout = TimeSpan.FromSeconds(10);
                                 cfg.TripThreshold = 10;
                             });

                         });

                             configurator.ReceiveEndpoint(e =>
                             {
                                 e.ConfigureConsumer<FaultConsumer>(context);


                             });



                         });

                     });

                     services.AddHostedService<MassTransitHostedService>();

                     services.AddHttpClient();

                 })
                 .ConfigureLogging((hostingContext, logging) => {

                     logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                     logging.AddConsole();
                 });
await builder.RunConsoleAsync();