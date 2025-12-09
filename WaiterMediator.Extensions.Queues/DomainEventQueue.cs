using System.Threading.Channels;
using WaiterMediator.Abstractions;

namespace WaiterMediator.Extensions.Queues;

public class DomainEventQueue : IDomainEventQueue
{
    private readonly Channel<INotification> _channel;

    public DomainEventQueue()
    {
        _channel = Channel.CreateUnbounded<INotification>();
    }

    public ValueTask EnqueueAsync(INotification domainEvent)
        => _channel.Writer.WriteAsync(domainEvent);

    public ChannelReader<INotification> Reader => _channel.Reader;
}