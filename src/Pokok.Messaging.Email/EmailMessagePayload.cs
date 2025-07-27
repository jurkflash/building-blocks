namespace Pokok.Messaging.Email
{
    public class EmailMessagePayload
    {
        public string ToEmail { get; set; } = default!;
        public string Subject { get; set; } = default!;
        public string Body { get; set; } = default!;
        public string TemplateKey { get; set; } = default!;
    }
}
