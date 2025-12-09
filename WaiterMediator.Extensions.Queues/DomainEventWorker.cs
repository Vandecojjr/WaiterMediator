using Microsoft.Extensions.Hosting;
using WaiterMediator.Abstractions;

namespace WaiterMediator.Extensions.Queues;

/// <summary>
/// BackgroundService que consome eventos do <see cref="DomainEventQueue"/> e publica-os via <see cref="IWaiter"/>.
/// </summary>
public class DomainEventWorker : BackgroundService
{
    private readonly DomainEventQueue _queue;
    private readonly IWaiter _waiter;

    /// <summary>
    /// Cria uma nova instância de <see cref="DomainEventWorker"/>.
    /// </summary>
    /// <param name="queue">Fila de domain events a ser consumida.</param>
    /// <param name="waiter">Mediador utilizado para publicar as notificações.</param>
    public DomainEventWorker(DomainEventQueue queue, IWaiter waiter)
    {
        _queue = queue;
        _waiter = waiter;
    }

    /// <summary>
    /// Loop de execução que consome eventos da fila e publica cada um via <see cref="IWaiter.Publish"/>.
    /// Exceções durante o processamento são exibidas no console, mas o loop continua para processar os próximos eventos.
    /// </summary>
    /// <param name="stoppingToken">Token para cancelamento do serviço em background.</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var domainEvent in _queue.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                await _waiter.Publish(domainEvent, stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar domain event: {ex}");
            }
        }
    }
}
