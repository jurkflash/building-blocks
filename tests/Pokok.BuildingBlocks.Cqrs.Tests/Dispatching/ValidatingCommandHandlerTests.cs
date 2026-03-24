using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Pokok.BuildingBlocks.Cqrs.Abstractions;
using Pokok.BuildingBlocks.Cqrs.Dispatching;
using Pokok.BuildingBlocks.Cqrs.Validation;
using Xunit;

namespace Pokok.BuildingBlocks.Cqrs.Dispatching;

public record CreateOrderCommand(string Name) : ICommand<bool>;

public class ValidatingCommandHandlerTests
{
    private readonly ICommandHandler<CreateOrderCommand, bool> _inner;

    public ValidatingCommandHandlerTests()
    {
        _inner = Substitute.For<ICommandHandler<CreateOrderCommand, bool>>();
    }

    [Fact]
    public async Task HandleAsync_WhenValidationPasses_DelegatesToInnerHandler()
    {
        var command = new CreateOrderCommand("Order1");
        _inner.HandleAsync(command, Arg.Any<CancellationToken>()).Returns(true);
        var validators = Enumerable.Empty<IValidator<CreateOrderCommand>>();
        var handler = new ValidatingCommandHandler<CreateOrderCommand, bool>(_inner, validators,
            NullLogger<ValidatingCommandHandler<CreateOrderCommand, bool>>.Instance);

        var result = await handler.HandleAsync(command, CancellationToken.None);

        Assert.True(result);
        await _inner.Received(1).HandleAsync(command, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenValidatorThrows_ThrowsValidationException()
    {
        var command = new CreateOrderCommand("");
        var validator = Substitute.For<IValidator<CreateOrderCommand>>();
        validator.When(v => v.Validate(command))
                 .Throw(new ValidationException(new[] { "Name is required." }));
        var handler = new ValidatingCommandHandler<CreateOrderCommand, bool>(
            _inner, new[] { validator }, NullLogger<ValidatingCommandHandler<CreateOrderCommand, bool>>.Instance);

        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => handler.HandleAsync(command, CancellationToken.None));

        Assert.Contains("Name is required.", exception.Errors);
    }

    [Fact]
    public async Task HandleAsync_WhenMultipleValidatorsFail_AggregatesAllErrors()
    {
        var command = new CreateOrderCommand("");
        var validator1 = Substitute.For<IValidator<CreateOrderCommand>>();
        validator1.When(v => v.Validate(command))
                  .Throw(new ValidationException(new[] { "Error1" }));
        var validator2 = Substitute.For<IValidator<CreateOrderCommand>>();
        validator2.When(v => v.Validate(command))
                  .Throw(new ValidationException(new[] { "Error2" }));
        var handler = new ValidatingCommandHandler<CreateOrderCommand, bool>(
            _inner, new[] { validator1, validator2 }, NullLogger<ValidatingCommandHandler<CreateOrderCommand, bool>>.Instance);

        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => handler.HandleAsync(command, CancellationToken.None));

        Assert.Contains("Error1", exception.Errors);
        Assert.Contains("Error2", exception.Errors);
    }

    [Fact]
    public async Task HandleAsync_WhenValidationFails_InnerHandlerIsNotCalled()
    {
        var command = new CreateOrderCommand("");
        var validator = Substitute.For<IValidator<CreateOrderCommand>>();
        validator.When(v => v.Validate(command))
                 .Throw(new ValidationException(new[] { "Invalid." }));
        var handler = new ValidatingCommandHandler<CreateOrderCommand, bool>(
            _inner, new[] { validator }, NullLogger<ValidatingCommandHandler<CreateOrderCommand, bool>>.Instance);

        await Assert.ThrowsAsync<ValidationException>(
            () => handler.HandleAsync(command, CancellationToken.None));

        await _inner.DidNotReceive().HandleAsync(Arg.Any<CreateOrderCommand>(), Arg.Any<CancellationToken>());
    }
}
