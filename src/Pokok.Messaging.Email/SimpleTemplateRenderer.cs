using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Pokok.Messaging.Email
{
    public class SimpleTemplateRenderer : ITemplateRenderer
    {
        private readonly EmailTemplatesOptions _templates;
        private readonly ILogger<SimpleTemplateRenderer>? _logger;

        public SimpleTemplateRenderer(IOptions<EmailTemplatesOptions> options, ILogger<SimpleTemplateRenderer>? logger = null)
        {
            _templates = options.Value;
            _logger = logger;
        }

        public (string Subject, string Body) Render(EmailTemplateKey templateKey, object model)
        {
            _logger?.LogDebug("Rendering template {TemplateKey}", templateKey);

            var templateOptions = templateKey switch
            {
                EmailTemplateKey.UserRegisteredConfirmation => _templates.UserRegisteredConfirmation,
                _ => throw new ArgumentOutOfRangeException(nameof(templateKey), templateKey, null)
            };

            string subject = templateOptions.Subject;
            string body = templateOptions.Body;

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
