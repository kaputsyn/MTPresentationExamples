using Courier.Components;
using Courier.Components.Authorize;
using Courier.Components.Deliver;
using Courier.Components.Purchase;
using Courier.Contracts;
using MassTransit;
using MassTransit.Definition;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton(KebabCaseEndpointNameFormatter.Instance);
builder.Services.AddMassTransit(x =>
{
    x.AddExecuteActivity<AuthorizeActivity, AuthorizeActivityArguments>();
    x.AddExecuteActivity<DeliverActivity, DeliverActivityArguments>();
    x.AddActivity<PurchaseActivity, PurchaseActivityArguments, PurchaseActivityLog>();

    x.AddConsumer<CourierSubmitOrderConsumer>();
    x.UsingRabbitMq((context, configurator) =>
    {

        configurator.ConfigureEndpoints(context);

    });
    x.AddRequestClient<SubmitOrder>();
}).AddMassTransitHostedService();


builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Courier.Api", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Courier.Api v1"));
}

app.UseAuthorization();

app.MapControllers();

app.Run();
