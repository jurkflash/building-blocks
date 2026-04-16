namespace Pokok.Messaging.Email
{
    /// <summary>
    /// Data transfer object representing an email message to be dispatched.
    /// Contains recipient lists (To, Cc, Bcc), subject, body content, and template key.
    /// </summary>
    public class EmailDispatchMessage
    {
        /// <summary>
        /// Gets or sets the list of primary recipient email addresses.
        /// </summary>
        public List<string> To { get; set; } = new();

        /// <summary>
        /// Gets or sets the list of carbon copy recipient email addresses.
        /// </summary>
        public List<string> Cc { get; set; } = new();

        /// <summary>
        /// Gets or sets the list of blind carbon copy recipient email addresses.
        /// </summary>
        public List<string> Bcc { get; set; } = new();

        /// <summary>
        /// Gets or sets the email subject line.
        /// </summary>
        public string Subject { get; set; } = default!;

        /// <summary>
        /// Gets or sets the email body content.
        /// </summary>
        public string Body { get; set; } = default!;

        /// <summary>
        /// Gets or sets a value indicating whether the body is HTML. Defaults to <c>true</c>.
        /// </summary>
        public bool IsHtml { get; set; } = true;

        /// <summary>
        /// Gets or sets the template key used to identify the email template.
        /// </summary>
        public string TemplateKey { get; set; } = default!;
    }
}
