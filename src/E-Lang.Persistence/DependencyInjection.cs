using Microsoft.Extensions.DependencyInjection;

namespace E_Lang.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        return services;
    }
}