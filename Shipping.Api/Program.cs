// var builder = WebApplication.CreateBuilder(args);
//
// // Add services to the container.
//
// builder.Services.AddControllers();
// // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();
//
// var app = builder.Build();
//
// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }
//
// app.UseHttpsRedirection();
//
// app.UseAuthorization();
//
// app.MapControllers();
//
// app.Run();

using Domain.Shipping.Repositories;
using Shipping.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. Adăugăm Controller-ele (ca să vadă ShippingController)
builder.Services.AddControllers();

// 2. Adăugăm Swagger (pentru interfața grafică de testare)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. (Opțional) Aici vom injecta Repository-urile mai târziu
// builder.Services.AddScoped<IShipmentRepository, ShipmentRepository>();

var app = builder.Build();

// 4. Configurăm pipeline-ul HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// IMPORTANT: Asta face ca API-ul să folosească Controller-ul pe care l-am scris
app.MapControllers(); 

app.Run();