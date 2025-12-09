using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TheMediator.Abstractions;

namespace TheMediator;

public static class TheMediatorDependency
{
    public static IServiceCollection AddTheMediator(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddScoped<IMediator, Mediator>();

        if (assemblies == null || assemblies.Length == 0)
            assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies) RegisterAssembly(services, assembly);
        return services;
    }

    private static void RegisterAssembly(IServiceCollection services, Assembly assembly)
    {
        var types = assembly.GetTypes();

        foreach (var type in types)
        {
            if (type.IsAbstract || type.IsInterface)
                continue;

            foreach (var @interface in type.GetInterfaces())
            {
                if (!@interface.IsGenericType)
                    continue;

                var definition = @interface.GetGenericTypeDefinition();

                if (definition == typeof(IRequestHandler<,>))
                {
                    services.AddScoped(@interface, type);
                    continue;
                }

                if (definition == typeof(INotificationHandler<>))
                {
                    services.AddScoped(@interface, type);
                    continue;
                }

                if (definition == typeof(IPipelineBehavior<,>))
                {
                    if (type.IsGenericTypeDefinition)
                    {
                        services.AddScoped(typeof(IPipelineBehavior<,>), type);
                    }
                    else
                    {
                        services.AddScoped(@interface, type);
                    }
                }
            }
        }
    }
}