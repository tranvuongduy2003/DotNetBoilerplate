using Application.UseCases;
using Domain.Abstractions.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
    {
        services
            .AddTransient<IAuthUseCase, AuthUseCase>();

        return services;
    }
}