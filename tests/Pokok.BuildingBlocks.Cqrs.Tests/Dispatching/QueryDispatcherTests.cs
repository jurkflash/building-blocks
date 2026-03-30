using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Pokok.BuildingBlocks.Cqrs.Abstractions;
using Pokok.BuildingBlocks.Cqrs.Dispatching;
using Xunit;

namespace Pokok.BuildingBlocks.Cqrs.Dispatching;

public record SampleQuery(string Filter) : IQuery<string>;

public class QueryDispatcherTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly QueryDispatcher _dispatcher;

    public QueryDispatcherTests()
    {
        _serviceProvider = Substitute.For<IServiceProvider>();
        _dispatcher = new QueryDispatcher(_serviceProvider, NullLogger<QueryDispatcher>.Instance);
    }

    [Fact]
    public async Task DispatchAsync_WithRegisteredHandler_ReturnsHandlerResult()
    {
        var query = new SampleQuery("active");
        var handler = Substitute.For<IQueryHandler<SampleQuery, string>>();
        handler.HandleAsync(query, Arg.Any<CancellationToken>()).Returns("data");

        _serviceProvider
            .GetService(typeof(IQueryHandler<SampleQuery, string>))
            .Returns(handler);

        var result = await _dispatcher.DispatchAsync<SampleQuery, string>(query);

        Assert.Equal("data", result);
    }

    [Fact]
    public async Task DispatchAsync_WithNoHandlerRegistered_ThrowsInvalidOperationException()
    {
        var query = new SampleQuery("active");

        _serviceProvider
            .GetService(typeof(IQueryHandler<SampleQuery, string>))
            .Returns(null);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _dispatcher.DispatchAsync<SampleQuery, string>(query));
    }

    [Fact]
    public async Task DispatchAsync_WithCancellationToken_PassesTokenToHandler()
    {
        var query = new SampleQuery("filter");
        var cts = new CancellationTokenSource();
        var handler = Substitute.For<IQueryHandler<SampleQuery, string>>();
        handler.HandleAsync(query, cts.Token).Returns("ok");

        _serviceProvider
            .GetService(typeof(IQueryHandler<SampleQuery, string>))
            .Returns(handler);

        await _dispatcher.DispatchAsync<SampleQuery, string>(query, cts.Token);

        await handler.Received(1).HandleAsync(query, cts.Token);
    }
}
