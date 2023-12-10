using E_Lang.Application.Common.Interfaces;
using E_Lang.Application.Common.Interfaces.Repositories;
using E_Lang.Persistence.Repositories;
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

        #region Repositories
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICollectionRepository, CollectionRepository>();
        services.AddScoped<IFlashcardRepository, FlashcardRepository>();
        services.AddScoped<IFlashcardBaseRepository, FlashcardBaseRepository>();
        
        #endregion
        
        return services;
    }
}