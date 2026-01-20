using Microsoft.EntityFrameworkCore;
using Domain.Sales.Repositories;
using Domain.Sales.Services;
using Domain.Sales.Workflows;
using Sales.Data;
using Sales.Data.Repositories;
using Sales.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. Configurare Baza de Date (EF Core)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SalesDbContext>(options =>
    options.UseSqlServer(connectionString));

// 3. Configurare Dependency Injection (Legăm Interfețele de Implementări)
builder.Services.AddScoped<IOrderRepository, SqlOrderRepository>();
builder.Services.AddScoped<IProductRepository, SqlProductRepository>();
builder.Services.AddTransient<IEventSender, ServiceBusTopicEventSender>();

// 4. Înregistrăm Workflow-ul
builder.Services.AddScoped<PlaceOrderWorkflow>();

var app = builder.Build();

// 5. Configurare Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SalesDbContext>();
    db.Database.EnsureCreated();
}

app.Run();