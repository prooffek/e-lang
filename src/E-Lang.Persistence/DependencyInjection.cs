using E_Lang.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace E_Lang.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new NullReferenceException("Connection string cannot be null or empty.");
        
        services.AddNpgsql<AppDbContext>(connectionString);
        services.AddScoped<IAppDbContext, AppDbContext>();
        return services;
    }
}