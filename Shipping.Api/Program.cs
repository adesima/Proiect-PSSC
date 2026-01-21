using Shipping.Data; // Presupunem că aici ai DbContext-ul
using Shipping.Data.Repositories; // Presupunem că aici ai implementarea repository-ului
using Domain.Shipping.Repositories;
using Domain.Shipping.Workflows; 
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Azure.Messaging.ServiceBus;
using Shipping.Api;
using Shipping.Api.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

// DbContext Shipping
builder.Services.AddDbContext<ShippingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ShippingConnection")),
    contextLifetime: ServiceLifetime.Singleton, 
    optionsLifetime: ServiceLifetime.Singleton);

// Service Bus Client 
builder.Services.AddSingleton<ServiceBusClient>(_ =>
    new ServiceBusClient(builder.Configuration["ServiceBus:ConnectionString"]));

// Background Listener
// Ascult mesajul de tip InvoicePaidMessage
builder.Services.AddHostedService<InvoicePaidListener>();

// Repository + Workflow
// Leg interfața din Domain de implementarea din Data
builder.Services.AddTransient<IShipmentRepository, ShipmentRepository>();
// Inregistrez Workflow-ul ca să poată fi injectat în Controller
builder.Services.AddTransient<ShippingWorkflow>();

// MVC Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Shipping.Api",
        Version = "v1"
    });
});

var app = builder.Build();

// Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();