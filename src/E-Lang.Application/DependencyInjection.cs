using System.Reflection;
using E_Lang.Application.Common.Mapster;
using E_Lang.Application.Interfaces;
using E_Lang.Application.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace E_Lang.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddMapster();
        services.AddMapsterConfig(Assembly.GetExecutingAssembly());
        services.AddScoped<IUserValidationService, UserValidationService>();
        services.AddScoped<ICollectionService, CollectionService>();
        
        return services;
    }
    
    private static void AddMapster(this IServiceCollection services)
    {
        MapsterConfig.ConfigureMapster();
    }

    private static void AddMapsterConfig(this IServiceCollection services, Assembly assembly)
    {
        MapsterConfig.ConfigureMapster(assembly);
    }
}