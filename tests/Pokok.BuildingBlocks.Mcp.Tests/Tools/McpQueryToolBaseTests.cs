using ModelContextProtocol;
using NSubstitute;
using Pokok.BuildingBlocks.Cqrs.Abstractions;
using Pokok.BuildingBlocks.Cqrs.Dispatching;
using Pokok.BuildingBlocks.Cqrs.Validation;
using Pokok.BuildingBlocks.Mcp.Tools;
using ModelContextProtocol.Server;
using System.ComponentModel;
using Xunit;

namespace Pokok.BuildingBlocks.Mcp.Tools;

public record GetItemQuery(Guid Id) : IQuery<string>;

[McpServerToolType]
public class GetItemTool : McpQueryToolBase<GetItemQuery, string>
{
    [McpServerTool, Description("Gets an item by ID.")]
    public static Task<string> GetItemAsync(
        IQueryDispatcher dispatcher,
        Guid id,
        CancellationToken ct)
        => ExecuteAsync(dispatcher, new GetItemQuery(id), ct);
}

public class McpQueryToolBaseTests
{
    private readonly IQueryDispatcher _dispatcher;

    public McpQueryToolBaseTests()
    {
        _dispatcher = Substitute.For<IQueryDispatcher>();
    }

    [Fact]
    public async Task ExecuteAsync_WithSuccessfulDispatch_ReturnsSerializedResult()
    {
        var id = Guid.NewGuid();
        _dispatcher
            .DispatchAsync<GetItemQuery, string>(Arg.Any<GetItemQuery>(), Arg.Any<CancellationToken>())
            .Returns("Widget");

        var result = await GetItemTool.GetItemAsync(_dispatcher, id, CancellationToken.None);

        Assert.Equal("\"Widget\"", result);
    }

    [Fact]
    public async Task ExecuteAsync_WhenValidationExceptionThrown_ThrowsMcpException()
    {
        var id = Guid.NewGuid();
        _dispatcher
            .When(d => d.DispatchAsync<GetItemQuery, string>(Arg.Any<GetItemQuery>(), Arg.Any<CancellationToken>()))
            .Throw(new ValidationException(new[] { "Id is invalid." }));

        var ex = await Assert.ThrowsAsync<McpException>(
            () => GetItemTool.GetItemAsync(_dispatcher, id, CancellationToken.None));

        Assert.Contains("Id is invalid.", ex.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WhenValidationExceptionHasMultipleErrors_AllErrorsIncludedInMessage()
    {
        var id = Guid.NewGuid();
        _dispatcher
            .When(d => d.DispatchAsync<GetItemQuery, string>(Arg.Any<GetItemQuery>(), Arg.Any<CancellationToken>()))
            .Throw(new ValidationException(new[] { "Error X", "Error Y" }));

        var ex = await Assert.ThrowsAsync<McpException>(
            () => GetItemTool.GetItemAsync(_dispatcher, id, CancellationToken.None));

        Assert.Contains("Error X", ex.Message);
        Assert.Contains("Error Y", ex.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNonValidationExceptionThrown_PropagatesOriginalException()
    {
        var id = Guid.NewGuid();
        _dispatcher
            .When(d => d.DispatchAsync<GetItemQuery, string>(Arg.Any<GetItemQuery>(), Arg.Any<CancellationToken>()))
            .Throw(new InvalidOperationException("Unexpected failure."));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => GetItemTool.GetItemAsync(_dispatcher, id, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_PassesQueryToDispatcher()
    {
        var id = Guid.NewGuid();
        _dispatcher
            .DispatchAsync<GetItemQuery, string>(Arg.Any<GetItemQuery>(), Arg.Any<CancellationToken>())
            .Returns("Widget");

        await GetItemTool.GetItemAsync(_dispatcher, id, CancellationToken.None);

        await _dispatcher.Received(1).DispatchAsync<GetItemQuery, string>(
            Arg.Is<GetItemQuery>(q => q.Id == id),
            Arg.Any<CancellationToken>());
    }
}
