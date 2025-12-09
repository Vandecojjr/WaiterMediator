using Moq;
using WaiterMediator.Abstractions;
using WaiterMediator.Extensions.Queues;

namespace WaiterMediator.Test;

public class DomainEventWorkerTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldPublishEventsFromQueue()
    {
        var mockWaiter = new Mock<IWaiter>();
        var queue = new DomainEventQueue();
        var worker = new DomainEventWorker(queue, mockWaiter.Object);
        var domainEvent = new Mock<INotification>().Object;

        await queue.EnqueueAsync(domainEvent);

        var cts = new CancellationTokenSource();
        cts.CancelAfter(100); 

        var workerTask = worker.StartAsync(cts.Token);
        await Task.Delay(50);
        cts.Cancel();

        mockWaiter.Verify(w => w.Publish(domainEvent, It.IsAny<CancellationToken>()), Times.Once);
        await workerTask;
    }

    [Fact]
    public async Task ExecuteAsync_ShouldHandleExceptionsAndContinue()
    {
        var mockWaiter = new Mock<IWaiter>();
        var queue = new DomainEventQueue();
        var worker = new DomainEventWorker(queue, mockWaiter.Object);
        var domainEvent = new Mock<INotification>().Object;

        mockWaiter
            .Setup(w => w.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Test exception"));

        await queue.EnqueueAsync(domainEvent);

        var cts = new CancellationTokenSource();
        cts.CancelAfter(100);

        var workerTask = worker.StartAsync(cts.Token);
        await Task.Delay(50);
        cts.Cancel();

        mockWaiter.Verify(w => w.Publish(domainEvent, It.IsAny<CancellationToken>()), Times.Once);
        await workerTask;
    }
}
