using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using WaiterMediator.Abstractions;

namespace WaiterMediator;

/// <summary>
/// Extensões de DI para registrar o mediador e descobrir handlers e behaviors por assembly.
/// </summary>
public static class WaiterMediatorDependency
{
    /// <summary>
    /// Registra o mediador e faz a varredura dos assemblies informados para registrar
    /// handlers de requisição, handlers de notificação e comportamentos de pipeline.
    /// </summary>
    /// <param name="services">Coleção de serviços a ser estendida.</param>
    /// <param name="assemblies">Assemblies a serem escaneados. Se vazio, usa os assemblies carregados no AppDomain.</param>
    /// <returns>A mesma instância de <see cref="IServiceCollection"/> para encadeamento.</returns>
    public static IServiceCollection AddWaiter(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddScoped<IWaiter, Waiter>();

        if (assemblies == null || assemblies.Length == 0)
            assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies) RegisterAssembly(services, assembly);
        return services;
    }

    
    /// <summary>
    /// Registra tipos encontrados no assembly conforme as interfaces genéricas suportadas:
    /// <see cref="IRequestHandler{TRequest, TResponse}"/>, <see cref="INotificationHandler{TNotification}"/>
    /// e <see cref="IPipelineBehavior{TRequest, TResponse}"/>.
    /// </summary>
    /// <param name="services">Coleção de serviços.</param>
    /// <param name="assembly">Assembly a ser registrado.</param>
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