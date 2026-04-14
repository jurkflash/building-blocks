# Pokok.Messaging.Email

> **Layer:** Infrastructure · **Dependencies:** Logging, Options · **Manifest:** [MODULE_MANIFEST.yaml](MODULE_MANIFEST.yaml)

## Purpose

Email messaging utilities with an extensible template rendering system. Composes email messages using placeholder-based substitution — does **not** send emails. Use the Outbox or Messaging module for delivery.

## Installation

```sh
dotnet add package Pokok.Messaging.Email
```

## Public API

| Type | Name | Description |
|------|------|-------------|
| Interface | `ITemplateRenderer` | Renders templates by replacing `{PropertyName}` placeholders with model values |
| Abstract | `EmailTemplateOptions` | Base class for email templates (Subject + Body) |
| Concrete | `EmailDispatchMessage` | Composed email with recipients, subject, body, HTML flag |
| Concrete | `SimpleTemplateRenderer` | Default renderer using reflection-based placeholder replacement |

## Behavioral Contracts

| Contract | Enforced By |
|----------|-------------|
| `{PropertyName}` placeholder syntax | `SimpleTemplateRenderer` replaces tokens matching model property names |
| Lenient rendering | Unmatched placeholders remain in output; logged at DEBUG, no exception |
| HTML by default | `EmailDispatchMessage.IsHtml` defaults to `true` |
| No predefined templates | `EmailTemplateOptions` is abstract — consumers define their own |

## Usage

### 1. Define Your Email Template
Create a template class in your application that inherits from `EmailTemplateOptions`:

```csharp
using Pokok.Messaging.Email;

public class WelcomeEmailTemplate : EmailTemplateOptions
{
    public override string Subject { get; set; } = "Welcome, {Name}!";
    public override string Body { get; set; } = "Hello {Name}, thanks for joining on {Date}.";
}
```

### 2. Register the Renderer
Register `ITemplateRenderer` in your DI container:

```csharp
services.AddScoped<ITemplateRenderer, SimpleTemplateRenderer>();
```

### 3. Render the Template
Inject `ITemplateRenderer` and render your template with a model:

```csharp
public class EmailService
{
    private readonly ITemplateRenderer _renderer;

    public EmailService(ITemplateRenderer renderer)
    {
        _renderer = renderer;
    }

    public EmailDispatchMessage CreateWelcomeEmail(string recipientEmail, string name)
    {
        var template = new WelcomeEmailTemplate();
        var (subject, body) = _renderer.Render(template, new { Name = name, Date = DateTime.Now.ToShortDateString() });

        return new EmailDispatchMessage
        {
            To = [recipientEmail],
            Subject = subject,
            Body = body
        };
    }
}
```

### 4. Load Templates from Configuration (Optional)
You can bind template values from `appsettings.json`:

```json
{
  "EmailTemplates": {
    "Welcome": {
      "Subject": "Welcome, {Name}!",
      "Body": "Hello {Name}, thanks for joining."
    }
  }
}
```

```csharp
services.Configure<WelcomeEmailTemplate>(configuration.GetSection("EmailTemplates:Welcome"));
```

Then inject `IOptions<WelcomeEmailTemplate>` to use the configured values.

## Failure Modes

| Failure | Trigger | Recovery |
|---------|---------|----------|
| Null template | Passing `null` to `Render()` | Provide a non-null template instance |
| Null model | Passing `null` model to `Render()` | Provide a non-null model object |
| Unmatched placeholders | Template has `{Placeholder}` with no matching model property | Ensure model properties match template placeholders |
| Non-string property | Model property `ToString()` returns unhelpful output | Override `ToString()` or use string properties on the model |

## Rules of Engagement

1. **Create concrete template classes** inheriting `EmailTemplateOptions` for each email type.
2. **Register `ITemplateRenderer`** as scoped or singleton in DI.
3. **Match placeholders exactly** — `{PropertyName}` is case-sensitive.
4. **Optionally bind templates from config** via `IOptions<T>` for runtime changes.
5. **This module does NOT send emails** — use the Outbox or Messaging module for delivery.

## Project Decisions

- `EmailTemplateOptions` is an abstract base class; consumers define their own templates.
- `SimpleTemplateRenderer` performs placeholder replacement using `{PropertyName}` syntax.
- No predefined templates in the shared library — templates are application-specific.
- Rendering is intentionally separated from delivery for testability and single responsibility.