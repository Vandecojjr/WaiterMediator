namespace WaiterMediator.Abstractions;

/// <summary>
/// Marca uma requisição que produz uma resposta do tipo <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">Tipo da resposta esperada.</typeparam>
public interface IRequest<TResponse>;