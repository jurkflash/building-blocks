using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Pokok.BuildingBlocks.Cqrs.Abstractions;
using Pokok.BuildingBlocks.Cqrs.Dispatching;
using Pokok.BuildingBlocks.Cqrs.Validation;
using Xunit;

namespace Pokok.BuildingBlocks.Cqrs.Dispatching;

public record GetOrderQuery(int OrderId) : IQuery<string>;

public class ValidatingQueryHandlerTests
{
    private readonly IQueryHandler<GetOrderQuery, string> _inner;

    public ValidatingQueryHandlerTests()
    {
        _inner = Substitute.For<IQueryHandler<GetOrderQuery, string>>();
    }

    [Fact]
    public async Task HandleAsync_WhenValidationPasses_DelegatesToInnerHandler()
    {
        var query = new GetOrderQuery(1);
        _inner.HandleAsync(query, Arg.Any<CancellationToken>()).Returns("order-data");
        var validators = Enumerable.Empty<IValidator<GetOrderQuery>>();
        var handler = new ValidatingQueryHandler<GetOrderQuery, string>(_inner, validators,
            NullLogger<ValidatingQueryHandler<GetOrderQuery, string>>.Instance);

        var result = await handler.HandleAsync(query, CancellationToken.None);

        Assert.Equal("order-data", result);
        await _inner.Received(1).HandleAsync(query, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenValidatorThrows_ThrowsValidationException()
    {
        var query = new GetOrderQuery(0);
        var validator = Substitute.For<IValidator<GetOrderQuery>>();
        validator.When(v => v.Validate(query))
                 .Throw(new ValidationException(new[] { "OrderId must be positive." }));
        var handler = new ValidatingQueryHandler<GetOrderQuery, string>(
            _inner, new[] { validator },
            NullLogger<ValidatingQueryHandler<GetOrderQuery, string>>.Instance);

        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => handler.HandleAsync(query, CancellationToken.None));

        Assert.Contains("OrderId must be positive.", exception.Errors);
    }

    [Fact]
    public async Task HandleAsync_WhenValidationFails_InnerHandlerIsNotCalled()
    {
        var query = new GetOrderQuery(0);
        var validator = Substitute.For<IValidator<GetOrderQuery>>();
        validator.When(v => v.Validate(query))
                 .Throw(new ValidationException(new[] { "Invalid." }));
        var handler = new ValidatingQueryHandler<GetOrderQuery, string>(
            _inner, new[] { validator },
            NullLogger<ValidatingQueryHandler<GetOrderQuery, string>>.Instance);

        await Assert.ThrowsAsync<ValidationException>(
            () => handler.HandleAsync(query, CancellationToken.None));

        await _inner.DidNotReceive().HandleAsync(Arg.Any<GetOrderQuery>(), Arg.Any<CancellationToken>());
    }
}
