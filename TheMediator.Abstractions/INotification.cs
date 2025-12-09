namespace TheMediator.Abstractions;

public interface INotification { }

public interface INotificationHandler<TNotification> where TNotification : INotification
{
    Task Handle(TNotification notification, CancellationToken cancellationToken);
}