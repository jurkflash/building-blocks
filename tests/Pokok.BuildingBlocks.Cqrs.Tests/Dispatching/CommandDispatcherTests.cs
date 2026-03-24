using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Pokok.BuildingBlocks.Cqrs.Abstractions;
using Pokok.BuildingBlocks.Cqrs.Dispatching;
using Xunit;

namespace Pokok.BuildingBlocks.Cqrs.Dispatching;

public record SampleCommand(string Value) : ICommand<string>;

public class CommandDispatcherTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly CommandDispatcher _dispatcher;

    public CommandDispatcherTests()
    {
        _serviceProvider = Substitute.For<IServiceProvider>();
        _dispatcher = new CommandDispatcher(_serviceProvider, NullLogger<CommandDispatcher>.Instance);
    }

    [Fact]
    public async Task DispatchAsync_WithRegisteredHandler_ReturnsHandlerResult()
    {
        var command = new SampleCommand("test");
        var handler = Substitute.For<ICommandHandler<SampleCommand, string>>();
        handler.HandleAsync(command, Arg.Any<CancellationToken>()).Returns("result");

        _serviceProvider
            .GetService(typeof(ICommandHandler<SampleCommand, string>))
            .Returns(handler);

        var result = await _dispatcher.DispatchAsync<SampleCommand, string>(command);

        Assert.Equal("result", result);
    }

    [Fact]
    public async Task DispatchAsync_WithNoHandlerRegistered_ThrowsInvalidOperationException()
    {
        var command = new SampleCommand("test");

        _serviceProvider
            .GetService(typeof(ICommandHandler<SampleCommand, string>))
            .Returns(null);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _dispatcher.DispatchAsync<SampleCommand, string>(command));
    }

    [Fact]
    public async Task DispatchAsync_WithCancellationToken_PassesTokenToHandler()
    {
        var command = new SampleCommand("test");
        var cts = new CancellationTokenSource();
        var handler = Substitute.For<ICommandHandler<SampleCommand, string>>();
        handler.HandleAsync(command, cts.Token).Returns("ok");

        _serviceProvider
            .GetService(typeof(ICommandHandler<SampleCommand, string>))
            .Returns(handler);

        await _dispatcher.DispatchAsync<SampleCommand, string>(command, cts.Token);

        await handler.Received(1).HandleAsync(command, cts.Token);
    }
}
