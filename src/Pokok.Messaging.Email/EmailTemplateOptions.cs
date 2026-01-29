namespace Pokok.Messaging.Email
{
    /// <summary>
    /// Base class for all email template options.
    /// Consumers should inherit from this class to define their own templates.
    /// </summary>
    public abstract class EmailTemplateOptions
    {
        public abstract string Subject { get; set; }
        public abstract string Body { get; set; }
    }
}
