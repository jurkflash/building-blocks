using Microsoft.Extensions.Options;

namespace Pokok.Messaging.Email
{
    public class SimpleTemplateRenderer : ITemplateRenderer
    {
        private readonly EmailTemplatesOptions _templates;

        public SimpleTemplateRenderer(IOptions<EmailTemplatesOptions> options)
        {
            _templates = options.Value;
        }

        public (string Subject, string Body) Render(EmailTemplateKey templateKey, object model)
        {
            var templateOptions = templateKey switch
            {
                EmailTemplateKey.UserRegisteredConfirmation => _templates.UserRegisteredConfirmationOptions,
                _ => throw new ArgumentOutOfRangeException(nameof(templateKey), templateKey, null)
            };

            string subject = templateOptions.Subject;
            string body = templateOptions.Body;

            foreach (var prop in model.GetType().GetProperties())
            {
                var value = prop.GetValue(model)?.ToString() ?? string.Empty;
                body = body.Replace($"{{{prop.Name}}}", value);
            }

            return (subject, body);
        }
    }
}
