using MassTransit;
using Microsoft.OpenApi.Models;
using Saga.Components.Consumers;
using Saga.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddMassTransit(cfg => 
{
    cfg.AddRequestClient<SubmitOrder>(new Uri("queue:submit-order"));
    cfg.AddRequestClient<CheckOrder>();

    cfg.UsingRabbitMq((context, configurator) => { configurator.ConfigureEndpoints(context); });
});
builder.Services.AddMassTransitHostedService();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Api", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1"));
}

app.UseAuthorization();

app.MapControllers();

app.Run();
