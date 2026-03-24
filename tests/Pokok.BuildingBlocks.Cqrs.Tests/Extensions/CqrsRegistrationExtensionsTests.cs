using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Pokok.BuildingBlocks.Cqrs.Abstractions;
using Pokok.BuildingBlocks.Cqrs.Dispatching;
using Pokok.BuildingBlocks.Cqrs.Events;
using Pokok.BuildingBlocks.Cqrs.Extensions;
using Pokok.BuildingBlocks.Cqrs.Validation;
using Xunit;

namespace Pokok.BuildingBlocks.Cqrs.Extensions;

public record RegisteredCommand(string Name) : ICommand<bool>;
public record RegisteredQuery(string Filter) : IQuery<string>;

public class SampleCommandHandler : ICommandHandler<RegisteredCommand, bool>
{
    public Task<bool> HandleAsync(RegisteredCommand command, CancellationToken cancellationToken)
        => Task.FromResult(true);
}

public class SampleQueryHandler : IQueryHandler<RegisteredQuery, string>
{
    public Task<string> HandleAsync(RegisteredQuery query, CancellationToken cancellationToken)
        => Task.FromResult("result");
}

public class SampleCommandValidator : IValidator<RegisteredCommand>
{
    public void Validate(RegisteredCommand request) { }
}

public class SampleQueryValidator : IValidator<RegisteredQuery>
{
    public void Validate(RegisteredQuery request) { }
}

public class CqrsRegistrationExtensionsTests
{
    [Fact]
    public void AddCommandHandlerWithValidator_WhenResolved_ReturnsValidatingWrapper()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddCommandHandler<RegisteredCommand, bool, SampleCommandHandler, SampleCommandValidator>();

        var provider = services.BuildServiceProvider();

        var handler = provider.GetRequiredService<ICommandHandler<RegisteredCommand, bool>>();

        Assert.IsType<ValidatingCommandHandler<RegisteredCommand, bool>>(handler);
    }

    [Fact]
    public void AddCommandHandlerWithoutValidator_WhenResolved_ReturnsConcreteHandler()
    {
        var services = new ServiceCollection();
        services.AddCommandHandler<RegisteredCommand, bool, SampleCommandHandler>();

        var provider = services.BuildServiceProvider();

        var handler = provider.GetRequiredService<ICommandHandler<RegisteredCommand, bool>>();

        Assert.IsType<SampleCommandHandler>(handler);
    }

    [Fact]
    public void AddQueryHandlerWithoutValidator_WhenResolved_ReturnsConcreteHandler()
    {
        var services = new ServiceCollection();
        services.AddQueryHandler<RegisteredQuery, string, SampleQueryHandler>();

        var provider = services.BuildServiceProvider();

        var handler = provider.GetRequiredService<IQueryHandler<RegisteredQuery, string>>();

        Assert.IsType<SampleQueryHandler>(handler);
    }

    [Fact]
    public void AddQueryHandlerWithValidator_WhenResolved_ReturnsValidatingWrapper()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddQueryHandler<RegisteredQuery, string, SampleQueryHandler, SampleQueryValidator>();

        var provider = services.BuildServiceProvider();

        var handler = provider.GetRequiredService<IQueryHandler<RegisteredQuery, string>>();

        Assert.IsType<ValidatingQueryHandler<RegisteredQuery, string>>(handler);
    }

    [Fact]
    public void AddDomainEventDispatcher_WhenResolved_ReturnsDomainEventDispatcher()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDomainEventDispatcher();

        var provider = services.BuildServiceProvider();

        var dispatcher = provider.GetRequiredService<IDomainEventDispatcher>();

        Assert.IsType<DomainEventDispatcher>(dispatcher);
    }
}
