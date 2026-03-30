using Pokok.Messaging.Email;
using Xunit;

namespace Pokok.Messaging.Email;

public class EmailDispatchMessageTests
{
    [Fact]
    public void Constructor_WithDefaultValues_InitializesCollections()
    {
        var message = new EmailDispatchMessage();

        Assert.NotNull(message.To);
        Assert.NotNull(message.Cc);
        Assert.NotNull(message.Bcc);
        Assert.True(message.IsHtml);
    }

    [Fact]
    public void Properties_WhenSet_ReturnCorrectValues()
    {
        var message = new EmailDispatchMessage
        {
            To = new List<string> { "to@example.com" },
            Cc = new List<string> { "cc@example.com" },
            Bcc = new List<string> { "bcc@example.com" },
            Subject = "Test Subject",
            Body = "<p>Hello</p>",
            IsHtml = false,
            TemplateKey = "welcome"
        };

        Assert.Equal("to@example.com", message.To[0]);
        Assert.Equal("cc@example.com", message.Cc[0]);
        Assert.Equal("bcc@example.com", message.Bcc[0]);
        Assert.Equal("Test Subject", message.Subject);
        Assert.Equal("<p>Hello</p>", message.Body);
        Assert.False(message.IsHtml);
        Assert.Equal("welcome", message.TemplateKey);
    }
}
