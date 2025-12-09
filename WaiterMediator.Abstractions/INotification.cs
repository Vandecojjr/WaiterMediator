namespace WaiterMediator.Abstractions;

/// <summary>
/// Representa uma notification/evento sem valor de retorno.
/// </summary>
public interface INotification { }

/// <summary>
/// Handler responsável por processar uma <see cref="INotification"/>.
/// </summary>
/// <typeparam name="TNotification">Tipo da notification.</typeparam>
public interface INotificationHandler<TNotification> where TNotification : INotification
{
    /// <summary>
    /// Processa a notification.
    /// </summary>
    /// <param name="notification">Instância da notification a ser processada.</param>
    /// <param name="cancellationToken">Token para cancelamento da operação.</param>
    Task Handle(TNotification notification, CancellationToken cancellationToken);
}