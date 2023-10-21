using Microsoft.Extensions.DependencyInjection;

namespace E_Lang.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}