using System.Reflection;
using E_Lang.Application.Common.Interfaces;
using Mapster;

namespace E_Lang.Application.Common.Mapster;

public class MapsterConfig
{
    public static void ConfigureMapster(Assembly? assembly = null)
    {
        TypeAdapterConfig.GlobalSettings.RequireExplicitMapping = true;
        TypeAdapterConfig.GlobalSettings.AllowImplicitSourceInheritance = true;
        TypeAdapterConfig.GlobalSettings.AllowImplicitDestinationInheritance = true;
        assembly ??= Assembly.GetExecutingAssembly();

        var types = assembly.GetExportedTypes()
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapper<>)))
            .ToList();

        foreach (var type in types)
        {
            var instance = Activator.CreateInstance(type);
            var methodInfo = type.GetMethod("Map")
                             ?? type.GetInterface("IMapper`1")!.GetMethod("Map");
            methodInfo?.Invoke(instance, new object[] {TypeAdapterConfig.GlobalSettings});
        }
    }
}