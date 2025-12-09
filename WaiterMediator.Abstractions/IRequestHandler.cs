namespace WaiterMediator.Abstractions;

/// <summary>
/// Marca uma requisição que produz uma resposta do tipo <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">Tipo da resposta esperada.</typeparam>
public interface IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Executa o processamento da requisição e retorna a resposta.
    /// </summary>
    /// <param name="request">Requisição a ser processada.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>A resposta da requisição.</returns>
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}