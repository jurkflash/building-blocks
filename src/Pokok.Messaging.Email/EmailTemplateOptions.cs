namespace Pokok.Messaging.Email
{
    /// <summary>
    /// Base class for all email template options.
    /// Consumers should inherit from this class to define their own templates.
    /// </summary>
    public abstract class EmailTemplateOptions
    {
        /// <summary>
        /// Gets or sets the email subject template.
        /// </summary>
        public abstract string Subject { get; set; }

        /// <summary>
        /// Gets or sets the email body template.
        /// </summary>
        public abstract string Body { get; set; }
    }
}
