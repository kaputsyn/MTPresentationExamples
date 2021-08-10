using Components;
using Contracts;
using MassTransit;
using MassTransit.Definition;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mediator
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMassTransit(cfg =>
            {




                //Send with queue creating with ezchange bindiungs
                // cfg.AddRequestClient<SubmitOrder>(new Uri($"queue:{KebabCaseEndpointNameFormatter.Instance.Consumer<SubmitOrderConsumer>()}"));

                //Sending to specific exchange without creating queue
                //cfg.AddRequestClient<SubmitOrder>(new Uri($"exchange:{KebabCaseEndpointNameFormatter.Instance.Consumer<SubmitOrderConsumer>()}"));


                //Publish
                cfg.AddRequestClient<SubmitOrder>();

                cfg.AddRequestClient<CheckOrder>();

                cfg.UsingRabbitMq((context, configurator) =>
                {
                    configurator.ConfigureEndpoints(context);

                    configurator.Host("localhost", "/", cfg =>
                    {
                        cfg.Username("guest");
                        cfg.Password("guest");

                    });
                });
            });

            services.AddMassTransitHostedService();

            services.AddOpenApiDocument();

            services.AddControllers();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });

            
        }
    }
}
