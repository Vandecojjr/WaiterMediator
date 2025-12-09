# WaiterMediator.Abstractions

Abstra??es do padr?o Mediator para facilitar CQRS e pub/sub em aplica??es .NET.

## Descri??o

Pacote leve com interfaces que definem contratos b?sicos para:
- Requisi??es e handlers (`IRequest<TResponse>`, `IRequestHandler<TRequest,TResponse>`)
- Notifica??es/Events e handlers (`INotification`, `INotificationHandler<TNotification>`)
- Comportamentos de pipeline (`IPipelineBehavior<TRequest,TResponse>`)
- Mediador (`IWaiter`)
- Fila de domain events para processamento ass?ncrono (`IDomainEventQueue`)

## Requisitos

- .NET 10.0 (TargetFramework: `net10.0`)

## Instala??o

Usando `dotnet`:

```bash
dotnet add package WaiterMediator.Abstractions