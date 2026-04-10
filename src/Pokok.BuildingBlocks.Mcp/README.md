# Pokok.BuildingBlocks.Mcp

## Purpose
Integrates the Pokok CQRS building blocks with the [Model Context Protocol (MCP) C# SDK](https://github.com/modelcontextprotocol/csharp-sdk), enabling your commands and queries to be called directly by AI/LLM agents.

## Installation
```
dotnet add package Pokok.BuildingBlocks.Mcp
```

## What's included

| Type | Description |
|---|---|
| `McpCommandToolBase<TCommand, TResult>` | Abstract base for MCP tools that dispatch a CQRS command |
| `McpQueryToolBase<TQuery, TResult>` | Abstract base for MCP tools that dispatch a CQRS query |
| `McpBuilderExtensions.WithCqrsDispatchers()` | Registers `ICommandDispatcher` and `IQueryDispatcher` for MCP tools |

## Example Usage

### 1. Register the MCP server with CQRS dispatchers

```csharp
builder.Services
    .AddMcpServer()
    .WithCqrsDispatchers()          // from Pokok.BuildingBlocks.Mcp
    .WithTools<PlaceOrderTool>()    // your concrete tool class
    .WithTools<GetOrderTool>();
```

### 2. Implement a command tool

```csharp
using ModelContextProtocol.Server;
using Pokok.BuildingBlocks.Cqrs.Dispatching;
using Pokok.BuildingBlocks.Mcp.Tools;
using System.ComponentModel;

[McpServerToolType]
public class PlaceOrderTool : McpCommandToolBase<PlaceOrderCommand, Guid>
{
    [McpServerTool, Description("Places a new order and returns the new order ID.")]
    public static Task<string> PlaceOrderAsync(
        IServiceProvider sp,
        [Description("Unique identifier of the product to order.")] string productId,
        [Description("Number of units to order.")] int quantity,
        CancellationToken ct)
    {
        var dispatcher = sp.GetRequiredService<ICommandDispatcher>();
        return ExecuteAsync(dispatcher, new PlaceOrderCommand(productId, quantity), ct);
    }
}
```

### 3. Implement a query tool

```csharp
[McpServerToolType]
public class GetOrderTool : McpQueryToolBase<GetOrderQuery, OrderDto>
{
    [McpServerTool, Description("Returns the details of an order by its ID.")]
    public static Task<string> GetOrderAsync(
        IServiceProvider sp,
        [Description("The order ID.")] Guid orderId,
        CancellationToken ct)
    {
        var dispatcher = sp.GetRequiredService<IQueryDispatcher>();
        return ExecuteAsync(dispatcher, new GetOrderQuery(orderId), ct);
    }
}
```

## How it works

- `ExecuteAsync` dispatches the command/query through the existing CQRS dispatcher.
- The result is serialized as JSON and returned to the AI client.
- If a `ValidationException` is thrown by the handler or validator, it is translated to a descriptive `McpException` so the AI receives a clear error message rather than an unhandled exception.

## Project Decisions
- MCP integration layer on top of `Pokok.BuildingBlocks.Cqrs`.
- Abstract base classes provide consistent error handling and result serialization without dictating the tool method signatures (AI parameter names/types vary per command).
