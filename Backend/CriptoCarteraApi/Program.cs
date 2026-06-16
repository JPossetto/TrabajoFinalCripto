using CriptoCarteraApi.Interfaces;
using CriptoCarteraApi.Models;
using CriptoCarteraApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<ITransaccionService, TransaccionService>();
builder.Services.AddScoped<IPrecioCryptoService, PrecioCryptoService>();
builder.Services.AddHttpClient();

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// esto crea la base si todavia no existe, asi no hay que pelearse tanto con migraciones.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

// habilita que la api pueda leer archivos comunes como el index.html, para que el cliente pueda manejar las rutas del frontend
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors("PermitirTodo");
app.MapControllers();

// si entra a una ruta que no es de la api, le devolvemos el index.html para que el cliente maneje la ruta (esto es para que funcione el router del frontend)
app.MapFallbackToFile("index.html");

app.Run();