namespace Pokok.Messaging.Email
{
    public class EmailDispatchMessage
    {
        public List<string> To { get; set; } = new();
        public List<string> Cc { get; set; } = new();
        public List<string> Bcc { get; set; } = new();

        public string Subject { get; set; } = default!;
        public string Body { get; set; } = default!;
        public bool IsHtml { get; set; } = true;

        public string TemplateKey { get; set; } = default!;
    }
}
