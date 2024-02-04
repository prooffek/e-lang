using System.Reflection;
using E_Lang.Application.Common.Mapster;
using E_Lang.Application.Interfaces;
using E_Lang.Application.Services;
using E_Lang.Application.Validators;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using FluentValidation;

namespace E_Lang.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddMapster();
        services.AddMapsterConfig(Assembly.GetExecutingAssembly());
        services.AddScoped<IMapper, ServiceMapper>();
        services.AddScoped<ICollectionService, CollectionService>();
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IFlashcardService, FlashcardService>();
        services.AddScoped<IAttemptStageService, AttemptStageService>();
        services.AddScoped<IExerciseService, ExerciseService>();
        services.AddValidatorsFromAssemblyContaining<CreateCollectionDtoValidator>();
        services.AddFluentValidationAutoValidation();
        
        return services;
    }
    
    private static IServiceCollection AddMapster(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
        
        return services;
    }

    private static void AddMapsterConfig(this IServiceCollection services, Assembly assembly)
    {
        MapsterConfig.ConfigureMapster(assembly);
    }
}