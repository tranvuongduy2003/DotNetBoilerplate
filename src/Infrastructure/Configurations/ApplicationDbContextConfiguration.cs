using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Configurations;

public static class ApplicationDbContextConfiguration
{
    public static IServiceCollection ConfigureApplicationDbContext(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnectionString");
        if (connectionString == null || string.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException("DefaultConnectionString is not configured.");
        services.AddDbContext<ApplicationDbContext>((provider, optionsBuilder) =>
        {
            var dateTrackingInterceptor =
                provider.GetService<DateTrackingInterceptor>()!;

            optionsBuilder
                .UseSqlServer(connectionString, builder =>
                    builder.MigrationsAssembly("EventHub.Persistence"))
                .AddInterceptors(
                    dateTrackingInterceptor
                );
        });
        return services;
    }
}