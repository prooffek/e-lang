using E_Lang.Application.Common.Interfaces;
using E_Lang.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace E_Lang.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        
        return services;
    }
}