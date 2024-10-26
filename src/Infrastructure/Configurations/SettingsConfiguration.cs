using Domain.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Configurations;

public static class SettingsConfiguration
{
    public static IServiceCollection ConfigureAppSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection(nameof(JwtOptions))
            .Get<JwtOptions>();
        services.AddSingleton<JwtOptions>(jwtOptions);

        var azureBlobStorage = configuration.GetSection(nameof(AzureBlobStorage))
            .Get<AzureBlobStorage>();
        services.AddSingleton<AzureBlobStorage>(azureBlobStorage);

        var emailSettings = configuration.GetSection(nameof(EmailSettings))
            .Get<EmailSettings>();
        services.AddSingleton<EmailSettings>(emailSettings);

        return services;
    }
}