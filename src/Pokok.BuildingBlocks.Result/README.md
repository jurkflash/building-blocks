# Pokok.BuildingBlocks.Result

A railway-oriented **Result pattern** for elegant error handling without exceptions. Provides `Result`, `Result<T>`, `ValidationResult`, and rich functional combinators.

## Installation

```bash
dotnet add package Pokok.BuildingBlocks.Result
```

## Quick Start

### Define Domain Errors

```csharp
public static class UserErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "User.NotFound", "The user was not found.");

    public static readonly Error EmailTaken = Error.Conflict(
        "User.EmailTaken", "The email address is already in use.");

    public static Error InvalidName(string name) => Error.Validation(
        "User.InvalidName", $"'{name}' is not a valid user name.");
}
```

### Return Results from Operations

```csharp
public Result<User> GetById(Guid id)
{
    var user = _repository.Find(id);
    return user is not null
        ? Result.Success(user)
        : Result.Failure<User>(UserErrors.NotFound);
}
```

### Use Implicit Conversions

```csharp
public Result<User> Create(string name, string email)
{
    if (string.IsNullOrWhiteSpace(name))
        return UserErrors.InvalidName(name);   // implicit Error → Result<User>

    var user = new User(name, email);
    return user;                                // implicit User → Result<User>
}
```

### Pattern Match on Results

```csharp
var message = result.Match(
    onSuccess: user => $"Welcome, {user.Name}!",
    onFailure: error => $"Error: {error.Description}");
```

### Chain Operations with Bind & Map

```csharp
Result<OrderConfirmation> PlaceOrder(Guid userId, Guid productId) =>
    GetUser(userId)
        .Bind(user => GetProduct(productId).Map(product => (user, product)))
        .Bind(pair => CreateOrder(pair.user, pair.product));
```

### Validate with Multiple Errors

```csharp
var validation = ResultExtensions.CombineAsValidation(
    ValidateName(request.Name),
    ValidateEmail(request.Email),
    ValidateAge(request.Age));

if (validation.IsFailure)
{
    foreach (var error in validation.Errors)
        Console.WriteLine(error);
}
```

### Convert Nullable Values

```csharp
User? user = _repository.Find(id);
Result<User> result = user.ToResult(UserErrors.NotFound);
```

## API Reference

| Type | Description |
|------|-------------|
| `Error` | Immutable record with `Code` and `Description` |
| `Result` | Success/failure without a value |
| `Result<T>` | Success/failure carrying a value of type `T` |
| `ValidationResult` | Failed result with multiple `Error` entries |
| `ValidationResult<T>` | Typed validation result with multiple errors |
| `ResultExtensions` | Async combinators, `Combine`, `ToResult` |
