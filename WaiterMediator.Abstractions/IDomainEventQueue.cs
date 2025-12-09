namespace WaiterMediator.Abstractions;

/// <summary>
/// Publica uma notification para todos os handlers registrados.
/// </summary>
/// <param name="notification">Notification a ser publicada.</param>
/// <param name="cancellationToken">Token de cancelamento opcional.</param>
public interface IDomainEventQueue
{
    /// <summary>
    /// Enfileira um domain event para processamento posterior.
    /// </summary>
    /// <param name="domainEvent">Domain event que implementa <see cref="INotification"/>.</param>
    /// <returns>Tarefa representando a operação de enfileiramento.</returns>
    ValueTask EnqueueAsync(INotification domainEvent);
}