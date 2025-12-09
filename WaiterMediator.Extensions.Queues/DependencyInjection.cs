using Microsoft.Extensions.DependencyInjection;
using WaiterMediator.Abstractions;

namespace WaiterMediator.Extensions.Queues;

/// <summary>
/// Extensões de IServiceCollection para registrar a fila de domain events e o worker.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra DomainEventQueue (singleton), expõe IDomainEventQueue e adiciona DomainEventWorker como HostedService.
    /// </summary>
    public static IServiceCollection AddDomainEventQueue(this IServiceCollection services)
    {
        services.AddSingleton<DomainEventQueue>();
        services.AddSingleton<IDomainEventQueue>(sp => sp.GetRequiredService<DomainEventQueue>());
        services.AddHostedService<DomainEventWorker>();
        return services;
    }
}