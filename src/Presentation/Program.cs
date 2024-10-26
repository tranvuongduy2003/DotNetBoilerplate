using Application;
using Infrastructure;
using Infrastructure.Configurations;
using Presentation.Extensions;
using Serilog;

var AppCors = "AppCors";

var builder = WebApplication.CreateBuilder(args);

Log.Information("Starting API up");

try
{
    builder.Host.UseSerilog(LoggingConfiguration.Configure);
    builder.Host.AddAppConfigurations();
    
    builder.Services.ConfigureInfrastructureServices(builder.Configuration, AppCors);
    builder.Services.ConfigureApplicationServices();

    var app = builder.Build();

    app.UseInfrastructure(AppCors);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, $"Unhandled exception: {ex.Message}");

    var type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal)) throw;
}
finally
{
    Log.Information("Shut down API complete");
    Log.CloseAndFlush();
}