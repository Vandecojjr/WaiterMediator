# WaiterMediator

Implementação leve do padrão Mediator para .NET, focada em cenários CQRS e Domain Events. Fornece abstrações e uma implementação simples para envio de requisições, publicação de notificações e processamento assíncrono de domain events.

## Visão geral

- Projeto: `WaiterMediator` (TargetFramework: `net10.0`)
- Pacote NuGet: `WaiterMediator` (versão contida em `WaiterMediator.csproj`)
- Objetivo: permitir envio/recebimento de mensagens (requests/notifications), composition de comportamentos (pipeline) e enfileiramento de eventos de domínio para processamento em background.

## Principais conceitos / APIs

- `IRequest<TResponse>` – marca uma requisição que produz `TResponse`.
- `IRequestHandler<TRequest, TResponse>` – processa uma requisição e retorna `TResponse`.
- `INotification` – marca uma notificação/evento sem retorno.
- `INotificationHandler<TNotification>` – processa notificações.
- `IPipelineBehavior<TRequest, TResponse>` – compõe lógica antes/depois do handler.
- `IWaiter` – mediador para `Send` (requisições) e `Publish` (notificações).
- `IDomainEventQueue` – fila para enfileirar `INotification` para processamento assíncrono (implementação disponível em `WaiterMediator.Extensions.Queues`).

## Requisitos

- .NET 10.0
- Recomenda-se usar o sistema de DI do `Microsoft.Extensions.DependencyInjection`.

## Instalação

Via NuGet:

    dotnet add package WaiterMediator --version 1.0.1

Ou referenciando os projetos na solução para desenvolvimento local.

## Registro no DI

Exemplo de registro do mediador e descoberta automática de handlers/behaviors:

    builder.Services.AddWaiter(); // varre assemblies carregados
    // ou informar assemblies explicitamente:
    builder.Services.AddWaiter(typeof(Program).Assembly);

O método `AddWaiter` registra:
- `IWaiter` (implementação `Waiter`)
- Implementações de `IRequestHandler<,>`, `INotificationHandler<>` e `IPipelineBehavior<,>` encontradas nos assemblies escaneados.

## Uso básico

Definir uma requisição e handler:

    public record GetUserQuery(int Id) : IRequest<UserDto>;

    public class GetUserHandler : IRequestHandler<GetUserQuery, UserDto>
    {
        public Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            // Implementação...
            throw new NotImplementedException();
        }
    }

Enviar uma requisição:

    var result = await waiter.Send(new GetUserQuery(1), cancellationToken);

Definir e publicar uma notification:

    public record UserCreatedEvent(int UserId) : INotification;

    public class UserCreatedHandler : INotificationHandler<UserCreatedEvent>
    {
        public Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
        {
            // Implementação...
            return Task.CompletedTask;
        }
    }

    await waiter.Publish(new UserCreatedEvent(1), cancellationToken);

Adicionar comportamento de pipeline:

    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, Func<Task<TResponse>> next)
        {
            // Antes do handler
            var response = await next();
            // Depois do handler
            return response;
        }
    }


## Boas práticas e observações

- Garanta que exista exatamente um `IRequestHandler<TRequest, TResponse>` registrado para cada `TRequest`.
- Handlers de notificação podem ser múltiplos (pub/sub).
- Comportamentos de pipeline são resolvidos e executados em ordem reversa da descoberta (último registrado executa primeiro).

## Contribuição

- Fork -> branch -> PR
- Issues e melhorias são bem-vindas.
- Adicione testes para novas features e mantenha consistência das abstrações.

## Licença

- MIT – confira a licença no repositório.

## Autor e repositório

- Autor: Vanderlei Junior
- Repositório: https://github.com/Vandecojjr/WaiterMediator
- Versão do pacote (atual): 1.0.1
