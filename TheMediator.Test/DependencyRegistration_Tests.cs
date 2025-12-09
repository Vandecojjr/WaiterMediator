using Microsoft.Extensions.DependencyInjection;
using TheMediator.Abstractions;

namespace TheMediator.Test;

public class DependencyRegistrationTests
{
    [Fact]
    public void Should_Register_Handlers_And_Pipelines()
    {
        var services = new ServiceCollection();

        services.AddTheMediator(typeof(FakeRequestHandler).Assembly);

        var provider = services.BuildServiceProvider();

        var handler = provider.GetService<IRequestHandler<FakeRequest, string>>();
        var mediator = provider.GetService<IMediator>();

        Assert.NotNull(handler);
        Assert.NotNull(mediator);
    }

    [Fact]
    public void Should_Register_Notification_Handlers()
    {
        var services = new ServiceCollection();

        services.AddTheMediator(typeof(FakeNotificationHandler).Assembly);

        var provider = services.BuildServiceProvider();

        var handlers = provider.GetServices<INotificationHandler<FakeNotification>>();

        Assert.Single(handlers);
    }
}