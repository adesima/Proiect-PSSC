using Billing.Data;
using Billing.Data.Repositories;
using Domain.Invoicing.Repositories;
using Domain.Invoicing.Workflows; // pune aici namespace-ul unde ai workflow-ul de Billing
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Azure.Messaging.ServiceBus;


var builder = WebApplication.CreateBuilder(args);

// 1. DbContext Billing
builder.Services.AddDbContext<BillingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BillingConnection")),
    contextLifetime: ServiceLifetime.Singleton,
    optionsLifetime: ServiceLifetime.Singleton);

// Service Bus Client
builder.Services.AddSingleton<ServiceBusClient>(_ =>
    new ServiceBusClient(builder.Configuration["ServiceBus:ConnectionString"]));

builder.Services.AddHostedService<OrderPlacedListener>();

// 2. Repository + workflow
builder.Services.AddTransient<IInvoicesRepository, InvoicesRepository>();
builder.Services.AddTransient<BillingWorkflow>(); // înlocuiește cu numele real al workflow-ului tău

// 3. MVC controllers
builder.Services.AddControllers();

// 4. Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Billing.Api",
        Version = "v1"
    });
});

var app = builder.Build();

/*using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BillingContext>();
    db.Database.EnsureCreated();
}*/

// 5. Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
