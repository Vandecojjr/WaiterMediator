using System.Threading.Channels;
using Moq;
using WaiterMediator.Abstractions;
using WaiterMediator.Extensions.Queues;

namespace WaiterMediator.Test;

public class DomainEventQueueTests
{
    [Fact]
    public async Task EnqueueAsync_ShouldAddEventToQueue()
    {
        var queue = new DomainEventQueue();
        var domainEvent = new Mock<INotification>().Object;

        await queue.EnqueueAsync(domainEvent);

        var reader = queue.Reader;
        Assert.True(await reader.WaitToReadAsync());
        Assert.Equal(domainEvent, await reader.ReadAsync());
    }

    [Fact]
    public void Reader_ShouldExposeChannelReader()
    {
        var queue = new DomainEventQueue();
        var reader = queue.Reader;

        Assert.NotNull(reader);
        Assert.IsType<ChannelReader<INotification>>(reader);
    }
}
