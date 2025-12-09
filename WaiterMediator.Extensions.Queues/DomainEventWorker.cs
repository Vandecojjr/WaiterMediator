using Microsoft.Extensions.Hosting;
using WaiterMediator.Abstractions;

namespace WaiterMediator.Extensions.Queues;

public class DomainEventWorker : BackgroundService
{
    private readonly DomainEventQueue _queue;
    private readonly IWaiter _waiter;

    public DomainEventWorker(DomainEventQueue queue, IWaiter waiter)
    {
        _queue = queue;
        _waiter = waiter;
    }

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
                throw;
            }
        }
    }
}