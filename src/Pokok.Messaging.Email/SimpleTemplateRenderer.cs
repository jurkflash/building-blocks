using Microsoft.Extensions.Logging;

namespace Pokok.Messaging.Email
{
    /// <summary>
    /// Default <see cref="ITemplateRenderer"/> that uses simple string replacement to substitute
    /// <c>{PropertyName}</c> placeholders in the email body with values from the model object.
    /// Logs warnings for unmatched placeholders.
    /// </summary>
    public class SimpleTemplateRenderer : ITemplateRenderer
    {
        private readonly ILogger<SimpleTemplateRenderer>? _logger;

        public SimpleTemplateRenderer(ILogger<SimpleTemplateRenderer>? logger = null)
        {
            _logger = logger;
        }

        public (string Subject, string Body) Render(EmailTemplateOptions template, object model)
        {
            ArgumentNullException.ThrowIfNull(template);

            _logger?.LogDebug("Rendering template");

            var subject = template.Subject;
            var body = template.Body;

            foreach (var prop in model.GetType().GetProperties())
            {
                var value = prop.GetValue(model)?.ToString() ?? string.Empty;

                if (!body.Contains($"{{{prop.Name}}}"))
                {
                    _logger?.LogDebug("Placeholder for property {Property} not found in body", prop.Name);
                }

                body = body.Replace($"{{{prop.Name}}}", value);
            }

            return (subject, body);
        }
    }
}
