using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Pokok.BuildingBlocks.Cqrs.Events;
using Pokok.BuildingBlocks.Domain.Events;
using Xunit;

namespace Pokok.BuildingBlocks.Cqrs.Events;

public sealed class OrderPlacedEvent : DomainEventBase { }

public class DomainEventDispatcherTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly DomainEventDispatcher _dispatcher;

    public DomainEventDispatcherTests()
    {
        _serviceProvider = Substitute.For<IServiceProvider>();
        _dispatcher = new DomainEventDispatcher(
            _serviceProvider, NullLogger<DomainEventDispatcher>.Instance);
    }

    [Fact]
    public async Task DispatchAsync_WithRegisteredHandler_InvokesHandler()
    {
        var domainEvent = new OrderPlacedEvent();
        var handler = Substitute.For<IDomainEventHandler<OrderPlacedEvent>>();

        _serviceProvider
            .GetService(typeof(IEnumerable<IDomainEventHandler<OrderPlacedEvent>>))
            .Returns(new[] { handler });

        await _dispatcher.DispatchAsync(new[] { domainEvent });

        await handler.Received(1).Handle(domainEvent, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DispatchAsync_WithNoHandlers_CompletesWithoutError()
    {
        var domainEvent = new OrderPlacedEvent();

        _serviceProvider
            .GetService(typeof(IEnumerable<IDomainEventHandler<OrderPlacedEvent>>))
            .Returns(Enumerable.Empty<IDomainEventHandler<OrderPlacedEvent>>());

        await _dispatcher.DispatchAsync(new[] { domainEvent });
    }

    [Fact]
    public async Task DispatchAsync_WithEmptyEventCollection_CompletesWithoutInvokingHandlers()
    {
        await _dispatcher.DispatchAsync(Enumerable.Empty<IDomainEvent>());

        _serviceProvider.DidNotReceive().GetService(Arg.Any<Type>());
    }

    [Fact]
    public async Task DispatchAsync_WhenHandlerThrows_PropagatesException()
    {
        var domainEvent = new OrderPlacedEvent();
        var handler = Substitute.For<IDomainEventHandler<OrderPlacedEvent>>();
        handler.Handle(domainEvent, Arg.Any<CancellationToken>())
               .Returns(Task.FromException(new InvalidOperationException("Handler error")));

        _serviceProvider
            .GetService(typeof(IEnumerable<IDomainEventHandler<OrderPlacedEvent>>))
            .Returns(new[] { handler });

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _dispatcher.DispatchAsync(new[] { domainEvent }));
    }
}
