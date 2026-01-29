namespace Pokok.Messaging.Email
{
    public interface ITemplateRenderer
    {
        /// <summary>
        /// Renders an email template using the specified template and model.
        /// </summary>
        /// <param name="template">The email template options.</param>
        /// <param name="model">The model containing placeholder values.</param>
        /// <returns>A tuple containing the rendered subject and body.</returns>
        (string Subject, string Body) Render(EmailTemplateOptions template, object model);
    }
}
