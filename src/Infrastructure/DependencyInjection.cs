using Domain.Abstractions.Services;
using Infrastructure.Configurations;
using Infrastructure.FilesSystem;
using Infrastructure.Mailler;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration, string appCors)
    {
        services.ConfigureControllers();
        services.ConfigureCors(appCors);
        services.ConfigureApplicationDbContext(configuration);
        services.ConfigureIdentity();
        services.ConfigureMapper();
        services.ConfigValidation();
        services.ConfigureSwagger();
        services.ConfigureAppSettings(configuration);
        services.ConfigureApplication();
        services.ConfigureAuthetication();
        services.ConfigureDependencyInjection();

        return services;
    }

    public static IServiceCollection ConfigureDependencyInjection(this IServiceCollection services)
    {
        services
            .AddTransient<ISerializeService, SerializeService>()
            .AddTransient<IFileService, AzureFileService>()
            .AddTransient<IEmailService, EmailService>()
            .AddTransient<ITokenService, TokenService>();

        return services;
    }
}