namespace WaiterMediator.Abstractions;

/// <summary>
/// Processa a notification.
/// </summary>
/// <param name="notification">Instância da notification a ser processada.</param>
/// <param name="cancellationToken">Token para cancelamento da operação.</param>
public interface IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    
    /// <summary>
    /// Intercepta a execução da requisição e chama o próximo delegate no pipeline.
    /// </summary>
    /// <param name="request">Requisição a ser processada.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <param name="next">Delegate que representa o próximo passo no pipeline e retorna a resposta.</param>
    /// <returns>A resposta resultante da execução.</returns>
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, Func<Task<TResponse>> next);
}