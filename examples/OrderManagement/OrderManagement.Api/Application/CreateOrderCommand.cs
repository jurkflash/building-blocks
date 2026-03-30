using OrderManagement.Api.Domain;
using Pokok.BuildingBlocks.Cqrs.Abstractions;
using Pokok.BuildingBlocks.Cqrs.Validation;
using Pokok.BuildingBlocks.Persistence.Abstractions;

namespace OrderManagement.Api.Application;

/// <summary>
/// Command to create a new order
/// </summary>
public record CreateOrderCommand(string CustomerName, decimal Amount) : ICommand<Guid>;

/// <summary>
/// Handler for CreateOrderCommand demonstrating CQRS pattern
/// </summary>
public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, Guid>
{
    private readonly IRepository<Order> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(
        IRepository<Order> repository,
        IUnitOfWork unitOfWork,
        ILogger<CreateOrderCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Guid> HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating order for customer: {CustomerName}", command.CustomerName);

        // Create domain entity (raises domain event)
        var order = Order.Create(command.CustomerName, command.Amount);

        // Persist
        await _repository.AddAsync(order);

        // Unit of Work commits transaction and dispatches domain events
        await _unitOfWork.CompleteAsync(cancellationToken);

        _logger.LogInformation("Order {OrderId} created successfully", order.Id);
        return order.Id;
    }
}

/// <summary>
/// Validator for CreateOrderCommand demonstrating validation pattern
/// </summary>
public class CreateOrderCommandValidator : IValidator<CreateOrderCommand>
{
    public void Validate(CreateOrderCommand request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.CustomerName))
            errors.Add("Customer name is required");

        if (request.CustomerName?.Length > 200)
            errors.Add("Customer name must be 200 characters or less");

        if (request.Amount <= 0)
            errors.Add("Amount must be greater than zero");

        if (request.Amount > 1_000_000)
            errors.Add("Amount cannot exceed 1,000,000");

        if (errors.Any())
            throw new ValidationException(errors);
    }
}
