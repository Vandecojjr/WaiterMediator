using Moq;
using TheMediator.Abstractions;

namespace TheMediator.Test;

public class MediatorSendTests
{
    [Fact]
    public async Task Send_Should_Invoke_Handler_And_Return_Response()
    {
        var serviceProvider = new Mock<IServiceProvider>();
        var handler = new FakeRequestHandler();
        var pipelines = Array.Empty<IPipelineBehavior<FakeRequest, string>>();

        serviceProvider
            .Setup(x => x.GetService(typeof(IRequestHandler<FakeRequest, string>)))
            .Returns(handler);

        serviceProvider
            .Setup(x => x.GetService(typeof(IEnumerable<IPipelineBehavior<FakeRequest, string>>)))
            .Returns(pipelines);

        var mediator = new Mediator(serviceProvider.Object);
        var response = await mediator.Send(new FakeRequest("TEST"));

        Assert.Equal("OK:TEST", response);
    }


    [Fact]
    public async Task Send_Should_Throw_If_Handler_Not_Found()
    {
        var serviceProvider = new Mock<IServiceProvider>();

        serviceProvider
            .Setup(x => x.GetService(It.IsAny<Type>()))
            .Returns(null);

        var mediator = new Mediator(serviceProvider.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            mediator.Send(new FakeRequest("TEST")));
    }
}