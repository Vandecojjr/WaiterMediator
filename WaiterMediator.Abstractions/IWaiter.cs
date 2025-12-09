namespace WaiterMediator.Abstractions;

/// <summary>
/// Abstração do mediador para envio de requisições e publicação de notificações.
/// </summary>
public interface IWaiter
{
    /// <summary>
    /// Envia uma requisição e obtém a resposta do tipo especificado.
    /// </summary>
    /// <typeparam name="TResponse">Tipo da resposta esperada.</typeparam>
    /// <param name="request">Requisição a ser enviada.</param>
    /// <param name="cancellationToken">Token de cancelamento opcional.</param>
    /// <returns>A resposta da requisição.</returns>
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Publica uma notification para todos os handlers registrados.
    /// </summary>
    /// <param name="notification">Notification a ser publicada.</param>
    /// <param name="cancellationToken">Token de cancelamento opcional.</param>
    Task Publish(INotification notification, CancellationToken cancellationToken = default);
}