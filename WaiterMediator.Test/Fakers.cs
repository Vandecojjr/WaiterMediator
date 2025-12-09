
using WaiterMediator.Abstractions;

public record FakeRequest(string Data) : IRequest<string>;

public class FakeRequestHandler : IRequestHandler<FakeRequest, string>
{
    public Task<string> Handle(FakeRequest request, CancellationToken cancellationToken)
        => Task.FromResult("OK:" + request.Data);
}

public record FakeNotification(string Message) : INotification;

public class FakeNotificationHandler : INotificationHandler<FakeNotification>
{
    public bool WasExecuted { get; private set; }

    public Task Handle(FakeNotification notification, CancellationToken cancellationToken)
    {
        WasExecuted = true;
        return Task.CompletedTask;
    }
}

public class FakePipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public bool Executed { get; private set; }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, Func<Task<TResponse>> next)
    {
        Executed = true;
        return await next();
    }
}