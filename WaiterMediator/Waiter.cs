using Microsoft.Extensions.DependencyInjection;
using WaiterMediator.Abstractions;

namespace WaiterMediator;

/// <summary>
/// Implementação do mediador responsável por enviar requisições e publicar notificações usando a
/// resolução de dependências do <see cref="IServiceProvider"/>.
/// </summary>
public class Waiter : IWaiter
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Cria uma nova instância de <see cref="Waiter"/>.
    /// </summary>
    /// <param name="serviceProvider">Provedor de serviços para resolver handlers e behaviors.</param>
    public Waiter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    /// <summary>
    /// Envia uma requisição e obtém a resposta do tipo especificado.
    /// </summary>
    /// <typeparam name="TResponse">Tipo da resposta esperada.</typeparam>
    /// <param name="request">Instância da requisição que implementa <see cref="IRequest{TResponse}"/>.</param>
    /// <param name="cancellationToken">Token para cancelamento da operação.</param>
    /// <returns>A resposta resultante da execução do handler.</returns>
    /// <exception cref="InvalidOperationException">Lançada quando nenhum handler é encontrado para a requisição.</exception>
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
    
    /// <summary>
    /// Publica uma notificação para todos os handlers registrados para o tipo de notificação.
    /// </summary>
    /// <param name="notification">Instância da notificação que implementa <see cref="INotification"/>.</param>
    /// <param name="cancellationToken">Token para cancelamento da operação.</param>
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
    
    /// <summary>
    /// Executa a cadeia de <see cref="IPipelineBehavior{TRequest, TResponse}"/> em torno do handler final.
    /// </summary>
    /// <typeparam name="TRequest">Tipo da requisição.</typeparam>
    /// <typeparam name="TResponse">Tipo da resposta.</typeparam>
    /// <param name="handlerExecution">Delegate que executa o handler final e retorna a resposta.</param>
    /// <param name="request">Requisição sendo processada.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>A resposta resultante da execução do pipeline e do handler.</returns>
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