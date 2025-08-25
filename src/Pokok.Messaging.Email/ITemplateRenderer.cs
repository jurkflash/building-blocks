namespace Pokok.Messaging.Email
{
    public interface ITemplateRenderer
    {
        (string Subject, string Body) Render(EmailTemplateKey templateKey, object model);
    }
}
