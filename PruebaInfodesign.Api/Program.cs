using PruebaInfodesign.Application.Interfaces;
using PruebaInfodesign.Application.Services;
using PruebaInfodesign.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions
            .CommandTimeout(180)
            .EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null)
    )
);

builder.Services.AddScoped<DbContext>(provider =>
    provider.GetRequiredService<Context>());

builder.Services.AddScoped<IConsumoService, ConsumoService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "PruebaInfodesign API",
        Version = "v1",
        Description = @"Endpoints:
        GET /api/Consumos/historico-por-tramos
        GET /api/Consumos/historico-por-cliente
        GET /api/Consumos/top20-peores-tramos"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PruebaInfodesign API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Lifetime.ApplicationStarted.Register(() =>
{
    Console.WriteLine("Proyecto Corriendo");
});

app.Run();
