# TheMediator

**TheMediator** é uma solução .NET que implementa o padrão Mediator para facilitar a comunicação entre componentes de uma aplicação, promovendo baixo acoplamento e alta coesão. A solução inclui suporte para requisições (CQRS), notificações (Domain Events) e processamento assíncrono de eventos com filas baseadas em `System.Threading.Channels`.

## Objetivo

O objetivo do **TheMediator** é fornecer uma implementação leve e extensível do padrão Mediator, com suporte a:
- **CQRS**: Envio de requisições e retorno de respostas.
- **Domain Events**: Publicação de eventos de domínio com múltiplos handlers.
- **Background Processing**: Processamento assíncrono de eventos com filas.

A solução é composta pelos seguintes projetos:
- **WaiterMediator**: Núcleo do Mediator, com abstrações e implementação principal.
- **WaiterMediator.Extensions.Queues**: Extensão para enfileiramento e processamento assíncrono de eventos.
- **WaiterMediator.Test**: Testes automatizados para garantir a qualidade e confiabilidade do código.

## Estrutura da Solução

- **WaiterMediator**  
  - Contém as interfaces principais (`IRequest`, `INotification`, `IWaiter`, etc.).
  - Implementação do mediador (`Waiter`) e suporte a pipelines (`IPipelineBehavior`).

- **WaiterMediator.Extensions.Queues**  
  - Adiciona suporte a filas para eventos de domínio (`DomainEventQueue`).
  - Inclui um `BackgroundService` para consumir eventos da fila (`DomainEventWorker`).

- **WaiterMediator.Test**  
  - Testes unitários para validar o comportamento do mediador e das extensões.

## Requisitos

- .NET 10.0 ou superior.
- Dependências:
  - `Microsoft.Extensions.DependencyInjection`
  - `Microsoft.Extensions.Hosting`
  - `WaiterMediator.Abstractions` (para extensões).

## Instalação

Adicione os pacotes necessários ao seu projeto:

```bash
dotnet add package WaiterMediator
dotnet add package WaiterMediator.Extensions.Queues
```

## Registro no DI

Configure o mediador e as extensões no `Program.cs`:

```csharp
var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Registro do Mediator
        services.AddWaiter(typeof(Program).Assembly);

        // Registro da extensão de filas
        services.AddDomainEventQueue();
    });

await builder.RunConsoleAsync();
```

## Exemplos de Uso

### 1. Requisições (CQRS)

Defina uma requisição e seu handler:

```csharp
public record GetUserQuery(int Id) : IRequest<UserDto>;

public class GetUserHandler : IRequestHandler<GetUserQuery, UserDto>
{
    public Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        // Simulação de busca de usuário
        return Task.FromResult(new UserDto(request.Id, "John Doe"));
    }
}
```

Envie a requisição usando o `IWaiter`:

```csharp
var user = await waiter.Send(new GetUserQuery(1), cancellationToken);
Console.WriteLine($"Usuário: {user.Name}");
```

### 2. Notificações (Domain Events)

Defina um evento e seus handlers:

```csharp
public record UserCreatedEvent(int UserId) : INotification;

public class UserCreatedHandler : INotificationHandler<UserCreatedEvent>
{
    public Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Usuário criado: {notification.UserId}");
        return Task.CompletedTask;
    }
}
```

Publique o evento:

```csharp
await waiter.Publish(new UserCreatedEvent(1), cancellationToken);
```

### 3. Processamento Assíncrono com Filas

Enfileire eventos para processamento em background:

```csharp
await domainEventQueue.EnqueueAsync(new UserCreatedEvent(42));
```

O `DomainEventWorker` consumirá a fila e publicará os eventos automaticamente.

### 4. Comportamentos de Pipeline

Adicione lógica antes/depois do handler:

```csharp
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, Func<Task<TResponse>> next)
    {
        Console.WriteLine($"Iniciando: {typeof(TRequest).Name}");
        var response = await next();
        Console.WriteLine($"Finalizando: {typeof(TRequest).Name}");
        return response;
    }
}
```

Registre o comportamento no DI:

```csharp
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
```

## Boas Práticas

- **Idempotência**: Certifique-se de que handlers de eventos sejam idempotentes, especialmente ao usar filas.
- **Observabilidade**: Adicione logs e métricas para monitorar o desempenho e o comportamento do sistema.
- **Testes**: Teste handlers, comportamentos e integração com o mediador.

## Contribuição

Contribuições são bem-vindas! Siga o fluxo abaixo:
1. Faça um fork do repositório.
2. Crie uma branch para sua feature ou correção (`feature/nome` ou `fix/nome`).
3. Adicione testes para validar suas alterações.
4. Abra um Pull Request descrevendo as mudanças.

## Licença

Este projeto está licenciado sob a licença MIT. Consulte o arquivo `LICENSE` para mais detalhes.

## Autor

- **Vanderlei Junior**  
- Repositório: [GitHub - Vandecojjr/WaiterMediator](https://github.com/Vandecojjr/WaiterMediator)
