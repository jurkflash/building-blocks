using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Pokok.Messaging.Email;

internal sealed class WelcomeEmailTemplate : EmailTemplateOptions
{
    public override string Subject { get; set; } = "Welcome, {Name}!";
    public override string Body { get; set; } = "Hello {Name}, thank you for joining!";
}

public class SimpleTemplateRendererTests
{
    private readonly SimpleTemplateRenderer _renderer;

    public SimpleTemplateRendererTests()
    {
        _renderer = new SimpleTemplateRenderer(NullLogger<SimpleTemplateRenderer>.Instance);
    }

    [Fact]
    public void Render_WithValidTemplateAndModel_ReplacesPlaceholders()
    {
        var template = new WelcomeEmailTemplate();
        var model = new { Name = "Alice" };

        var (subject, body) = _renderer.Render(template, model);

        Assert.Equal("Welcome, {Name}!", subject);
        Assert.Equal("Hello Alice, thank you for joining!", body);
    }

    [Fact]
    public void Render_WithMultiplePlaceholders_ReplacesAllOccurrences()
    {
        var template = new WelcomeEmailTemplate
        {
            Body = "Hello {Name}, your code is {Code}."
        };
        var model = new { Name = "Bob", Code = "XYZ123" };

        var (_, body) = _renderer.Render(template, model);

        Assert.Equal("Hello Bob, your code is XYZ123.", body);
    }

    [Fact]
    public void Render_WithNullTemplate_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => _renderer.Render(null!, new { Name = "Alice" }));
    }

    [Fact]
    public void Render_WithModelWithNoMatchingProperties_LeavesPlaceholderUnchanged()
    {
        var template = new WelcomeEmailTemplate
        {
            Body = "Hello {FirstName}!"
        };
        var model = new { LastName = "Doe" };

        var (_, body) = _renderer.Render(template, model);

        Assert.Equal("Hello {FirstName}!", body);
    }

    [Fact]
    public void Render_WithoutLogger_DoesNotThrow()
    {
        var renderer = new SimpleTemplateRenderer();
        var template = new WelcomeEmailTemplate();
        var model = new { Name = "Alice" };

        var (_, body) = renderer.Render(template, model);

        Assert.Equal("Hello Alice, thank you for joining!", body);
    }
}
