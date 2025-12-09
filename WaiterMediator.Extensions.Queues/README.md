# WaiterMediator.Extensions.Queues

Fila de Domain Events baseada em System.Threading.Channels integrada ao WaiterMediator. Objetivo: enfileirar INotification para processamento em background por um worker que publica eventos via IWaiter.

## Principais componentes e responsabilidades

- DomainEventQueue
    - Implementa uma fila baseada em Channel<INotification>.
    - EnqueueAsync(INotification) para enfileiramento assíncrono.
    - Reader expõe ChannelReader<INotification> para consumo.

- DomainEventWorker
    - BackgroundService que consome DomainEventQueue.Reader e chama IWaiter.Publish(notification, ct) para cada item.

- AddDomainEventQueue (extensão de DI)
    - Registra DomainEventQueue como singleton.
    - Mapeia IDomainEventQueue para a instância singleton.
    - Adiciona DomainEventWorker como HostedService.

## Requisitos técnicos

- .NET 10.0
- WaiterMediator.Abstractions
- Microsoft.Extensions.Hosting
- Microsoft.Extensions.DependencyInjection

## Instalação

Via dotnet CLI:

```bash
dotnet add package WaiterMediator.Extensions.Queues
```

## Registro no DI

Exemplo mínimo de registro:

```csharp
// Program.cs
builder.Services.AddWaiter(typeof(Program).Assembly);
builder.Services.AddDomainEventQueue();
```

## Exemplos de eventos e handlers

Evento:

```csharp
public record UserCreatedEvent(int UserId) : INotification;
```

Handler síncrono:

```csharp
public class UserCreatedHandler : INotificationHandler<UserCreatedEvent>
{
    public Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"UserCreated: {notification.UserId}");
        return Task.CompletedTask;
    }
}
```

Handler assíncrono:

```csharp
public class UserCreatedAsyncHandler : INotificationHandler<UserCreatedEvent>
{
    public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        await Task.Delay(100, cancellationToken);
        Console.WriteLine($"Processed async: {notification.UserId}");
    }
}
```

## Exemplo de enfileiramento (produtor)

```csharp
// produtor: enfileira um domain event
await domainEventQueue.EnqueueAsync(new UserCreatedEvent(42));
```

Observação: o DomainEventWorker consome a fila e chama IWaiter.Publish(notification, ct), que resolve e executa os handlers registrados.

## Exemplo completo mínimo de Program.cs

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

return Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddWaiter(typeof(Program).Assembly);
        services.AddDomainEventQueue();
        // registrar demais serviços
    })
    .RunConsoleAsync();
```

## Boas práticas e considerações

- Idempotência: handlers devem ser idempotentes; eventos podem ser reprocessados.
- Canais bounded vs unbounded: prefira bounded para controlar memória e aplicar backpressure; unbounded simplifica mas pode consumir muita memória em picos.
- Retry e observability: aplicar políticas de retry/exponential backoff quando apropriado; expor métricas (tamanho da fila, latência, falhas).
- Ordem e concorrência: um worker preserva ordem; múltiplos consumidores aumentam concorrência e podem alterar ordem.

## API resumida

- DomainEventQueue
    - ValueTask EnqueueAsync(INotification domainEvent)
    - ChannelReader<INotification> Reader

- DomainEventWorker
    - BackgroundService que consome Reader e chama IWaiter.Publish

- AddDomainEventQueue()
    - Extensão IServiceCollection que registra DomainEventQueue (singleton), mapeia IDomainEventQueue e adiciona o hosted service

## Debug / Desenvolvimento

- Habilitar logs do host para acompanhar o worker.
- Em testes, injetar IWaiter mock/spy para verificar Publish.
- Instrumentar métricas básicas (tamanho da fila, taxa de consumo) para diagnóstico.

## Contribuição

Fluxo recomendado:
1. Fork -> criar branch feature/x ou fix/x.
2. Implementar mudança com testes automatizados.
3. Abrir Pull Request com descrição e impacto.

## Licença e autor

- Autor: Vanderlei Junior
- Licença: MIT
- Repositório: https://github.com/Vandecojjr/WaiterMediator
