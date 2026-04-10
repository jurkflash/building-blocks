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

public record CreateItemCommand(string Name) : ICommand<Guid>;

[McpServerToolType]
public class CreateItemTool : McpCommandToolBase<CreateItemCommand, Guid>
{
    [McpServerTool, Description("Creates a new item.")]
    public static Task<string> CreateItemAsync(
        ICommandDispatcher dispatcher,
        string name,
        CancellationToken ct)
        => ExecuteAsync(dispatcher, new CreateItemCommand(name), ct);
}

public class McpCommandToolBaseTests
{
    private readonly ICommandDispatcher _dispatcher;

    public McpCommandToolBaseTests()
    {
        _dispatcher = Substitute.For<ICommandDispatcher>();
    }

    [Fact]
    public async Task ExecuteAsync_WithSuccessfulDispatch_ReturnsSerializedResult()
    {
        var expectedId = Guid.NewGuid();
        _dispatcher
            .DispatchAsync<CreateItemCommand, Guid>(Arg.Any<CreateItemCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedId);

        var result = await CreateItemTool.CreateItemAsync(_dispatcher, "Widget", CancellationToken.None);

        Assert.Equal($"\"{expectedId}\"", result);
    }

    [Fact]
    public async Task ExecuteAsync_WhenValidationExceptionThrown_ThrowsMcpException()
    {
        _dispatcher
            .When(d => d.DispatchAsync<CreateItemCommand, Guid>(Arg.Any<CreateItemCommand>(), Arg.Any<CancellationToken>()))
            .Throw(new ValidationException(new[] { "Name is required." }));

        var ex = await Assert.ThrowsAsync<McpException>(
            () => CreateItemTool.CreateItemAsync(_dispatcher, "", CancellationToken.None));

        Assert.Contains("Name is required.", ex.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WhenValidationExceptionHasMultipleErrors_AllErrorsIncludedInMessage()
    {
        _dispatcher
            .When(d => d.DispatchAsync<CreateItemCommand, Guid>(Arg.Any<CreateItemCommand>(), Arg.Any<CancellationToken>()))
            .Throw(new ValidationException(new[] { "Error A", "Error B" }));

        var ex = await Assert.ThrowsAsync<McpException>(
            () => CreateItemTool.CreateItemAsync(_dispatcher, "", CancellationToken.None));

        Assert.Contains("Error A", ex.Message);
        Assert.Contains("Error B", ex.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNonValidationExceptionThrown_PropagatesOriginalException()
    {
        _dispatcher
            .When(d => d.DispatchAsync<CreateItemCommand, Guid>(Arg.Any<CreateItemCommand>(), Arg.Any<CancellationToken>()))
            .Throw(new InvalidOperationException("Unexpected failure."));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => CreateItemTool.CreateItemAsync(_dispatcher, "Widget", CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_PassesCommandToDispatcher()
    {
        var expectedId = Guid.NewGuid();
        _dispatcher
            .DispatchAsync<CreateItemCommand, Guid>(Arg.Any<CreateItemCommand>(), Arg.Any<CancellationToken>())
            .Returns(expectedId);

        await CreateItemTool.CreateItemAsync(_dispatcher, "Widget", CancellationToken.None);

        await _dispatcher.Received(1).DispatchAsync<CreateItemCommand, Guid>(
            Arg.Is<CreateItemCommand>(c => c.Name == "Widget"),
            Arg.Any<CancellationToken>());
    }
}
