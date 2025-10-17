using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PruebaInfodesign.Infrastructure.Data;
using PruebaInfodesign.Infrastructure.Utilities;

Console.WriteLine("Importar excel");

try
{
    // Configuración
    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();

    var connectionString = configuration.GetConnectionString("DefaultConnection");
    var rutaArchivo = configuration["ImportSettings:RutaArchivoExcel"];

    if (string.IsNullOrEmpty(connectionString))
    {
        return;
    }

    if (string.IsNullOrEmpty(rutaArchivo))
    {
        return;
    }

    var optionsBuilder = new DbContextOptionsBuilder<Context>();
    optionsBuilder.UseSqlServer(connectionString);

    using var context = new Context(optionsBuilder.Options);

    if (!await context.Database.CanConnectAsync())
    {
        return;
    }

    var importer = new ExcelImporter(context);
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();

    var resultado = await importer.ImportarDesdeExcel(rutaArchivo);

    stopwatch.Stop();

    if (resultado.Exitoso)
    {
        Console.WriteLine("Importacion exitosa");
    }
    else
    {
        Console.WriteLine($"Error: {resultado.Mensaje}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Mensaje: {ex.Message}");
    Console.WriteLine($"Tipo: {ex.GetType().Name}");

    if (ex.InnerException != null)
    {
        Console.WriteLine($"Error: {ex.InnerException.Message}");
    }
}

Console.ReadKey();