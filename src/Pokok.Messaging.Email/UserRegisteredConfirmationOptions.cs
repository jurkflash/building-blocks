namespace Pokok.Messaging.Email
{
    public class UserRegisteredConfirmationOptions
    {
        public const string SectionName = "UserRegisteredConfirmation";
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
