using System.Threading.Channels;
using WaiterMediator.Abstractions;

namespace WaiterMediator.Extensions.Queues;

/// <summary>
/// Fila baseada em <see cref="Channel{T}"/> para enfileirar <see cref="INotification"/> (domain events)
/// para processamento assíncrono por um worker em background.
/// </summary>
public class DomainEventQueue : IDomainEventQueue
{
    private readonly Channel<INotification> _channel;

    
    /// <summary>
    /// Cria uma nova instância de <see cref="DomainEventQueue"/> usando um channel não limitado.
    /// </summary>
    public DomainEventQueue()
    {
        _channel = Channel.CreateUnbounded<INotification>();
    }

    
    /// <summary>
    /// Enfileira um domain event para processamento posterior.
    /// </summary>
    /// <param name="domainEvent">Domain event que implementa <see cref="INotification"/>.</param>
    /// <returns>Tarefa representando a operação de enfileiramento.</returns>
    public ValueTask EnqueueAsync(INotification domainEvent)
        => _channel.Writer.WriteAsync(domainEvent);

    /// <summary>
    /// Leitor do channel usado pelo worker para consumir os eventos enfileirados.
    /// </summary>
    public ChannelReader<INotification> Reader => _channel.Reader;
}