using TheMediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace TheMediator;

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        var handler = _serviceProvider.GetService(handlerType);

        if (handler == null)
            throw new InvalidOperationException($"Handler não encontrado para {request.GetType().Name}");

        var method = handlerType.GetMethod("Handle")!;

        return await ExecutePipeline((ct) =>
            (Task<TResponse>)method.Invoke(handler, new object[] { request, ct })!,
            request,
            cancellationToken);
    }

    public async Task Publish(INotification notification, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(INotificationHandler<>).MakeGenericType(notification.GetType());
        var handlers = _serviceProvider.GetServices(handlerType);

        foreach (var handler in handlers)
        {
            var method = handlerType.GetMethod("Handle")!;
            await (Task)method.Invoke(handler, new object[] { notification, cancellationToken })!;
        }
    }

    private async Task<TResponse> ExecutePipeline<TRequest, TResponse>(
        Func<CancellationToken, Task<TResponse>> handlerExecution,
        TRequest request,
        CancellationToken cancellationToken)
        where TRequest : IRequest<TResponse>
    {
        var behaviors = _serviceProvider
            .GetService<IEnumerable<IPipelineBehavior<TRequest, TResponse>>>() ??
                        Enumerable.Empty<IPipelineBehavior<TRequest, TResponse>>()
            .Reverse()
            .ToList();

        Func<Task<TResponse>> next = () => handlerExecution(cancellationToken);

        foreach (var behavior in behaviors)
        {
            var current = next;
            next = () => behavior.Handle(request, cancellationToken, current);
        }

        return await next();
    }
}