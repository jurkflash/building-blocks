# Pokok.Messaging.Email

## Purpose
Email messaging utilities for Pokok Platform. Provides an extensible template rendering system.

## Installation
Install via NuGet:
```
dotnet add package Pokok.Messaging.Email
```

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

## Project Decisions
- `EmailTemplateOptions` is an abstract base class; consumers define their own templates.
- `SimpleTemplateRenderer` performs placeholder replacement using `{PropertyName}` syntax.
- No predefined templates in the shared library — templates are application-specific.