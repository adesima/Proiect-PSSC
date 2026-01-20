// // // var builder = WebApplication.CreateBuilder(args);
// // //
// // // // Add services to the container.
// // //
// // // builder.Services.AddControllers();
// // // // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// // // builder.Services.AddOpenApi();
// // //
// // // var app = builder.Build();
// // //
// // // // Configure the HTTP request pipeline.
// // // if (app.Environment.IsDevelopment())
// // // {
// // //     app.MapOpenApi();
// // // }
// // //
// // // app.UseHttpsRedirection();
// // //
// // // app.UseAuthorization();
// // //
// // // app.MapControllers();
// // //
// // // app.Run();
// //
// // using Domain.Shipping.Repositories;
// // using Shipping.Data.Repositories;
// //
// // var builder = WebApplication.CreateBuilder(args);
// //
// // // 1. Adăugăm Controller-ele (ca să vadă ShippingController)
// // builder.Services.AddControllers();
// //
// // // 2. Adăugăm Swagger (pentru interfața grafică de testare)
// // builder.Services.AddEndpointsApiExplorer();
// // builder.Services.AddSwaggerGen();
// //
// // // 3. (Opțional) Aici vom injecta Repository-urile mai târziu
// // // builder.Services.AddScoped<IShipmentRepository, ShipmentRepository>();
// //
// // var app = builder.Build();
// //
// // // 4. Configurăm pipeline-ul HTTP
// // if (app.Environment.IsDevelopment())
// // {
// //     app.UseSwagger();
// //     app.UseSwaggerUI();
// // }
// //
// // // IMPORTANT: Asta face ca API-ul să folosească Controller-ul pe care l-am scris
// // app.MapControllers(); 
// //
// // app.Run();
//
//
// // using Microsoft.EntityFrameworkCore;
// using Domain.Shipping.Repositories;
// using Shipping.Data;
// using Shipping.Data.Repositories;
//
// var builder = WebApplication.CreateBuilder(args);
//
// // --- CONFIGURARE SERVICII ---
//
// // 1. Controller
// builder.Services.AddControllers();
//
// // 2. Swagger (pentru testare)
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
//
// // 3. Baza de Date (Folosim In-Memory pentru Demo rapid)
// // builder.Services.AddDbContext<ShippingDbContext>(options =>
// //     options.UseInMemoryDatabase("ShippingDb"));
//
// // ⚠️ Dacă vrei SQL Server real (Azure), comentează linia de sus și decomenteaz-o pe asta:
// /*
// builder.Services.AddDbContext<ShippingDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// */
//
// // 4. Injectare Repository
// builder.Services.AddScoped<IShipmentRepository, ShipmentRepository>();
//
// var app = builder.Build();
//
// // --- CONFIGURARE PIPELINE HTTP ---
//
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }
//
// app.MapControllers();
//
// app.Run();


using Shipping.Data; // Presupunem că aici ai DbContext-ul
using Shipping.Data.Repositories; // Presupunem că aici ai implementarea repository-ului
using Domain.Shipping.Repositories;
using Domain.Shipping.Workflows; 
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Azure.Messaging.ServiceBus;
using Shipping.Api;
using Shipping.Api.BackgroundServices;

// using Shipping.Api.BackgroundServices; // Aici ar trebui să fie InvoicePaidListener

var builder = WebApplication.CreateBuilder(args);

// 1. DbContext Shipping
// Atenție: Asigură-te că ai "ShippingConnection" în appsettings.json
builder.Services.AddDbContext<ShippingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ShippingConnection")),
    contextLifetime: ServiceLifetime.Singleton, // Păstrat Singleton ca la colegul tău, deși Scoped e standard
    optionsLifetime: ServiceLifetime.Singleton);

// Service Bus Client (Rămâne la fel, doar string-ul de conectare e același de obicei)
builder.Services.AddSingleton<ServiceBusClient>(_ =>
    new ServiceBusClient(builder.Configuration["ServiceBus:ConnectionString"]));

// 2. Background Listener
// Tu asculți mesajul "InvoicePaid" ca să începi livrarea
builder.Services.AddHostedService<InvoicePaidListener>();

// 3. Repository + Workflow
// Legăm interfața din Domain de implementarea din Data
builder.Services.AddTransient<IShipmentRepository, ShipmentRepository>();
// Înregistrăm Workflow-ul ca să poată fi injectat în Controller
builder.Services.AddTransient<ShippingWorkflow>();

// 4. MVC Controllers
builder.Services.AddControllers();

// 5. Swagger
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

// Inițializare Bază de date (Opțional, bun pentru teste)
/*using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ShippingContext>();
    db.Database.EnsureCreated();
}*/

// 6. Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();