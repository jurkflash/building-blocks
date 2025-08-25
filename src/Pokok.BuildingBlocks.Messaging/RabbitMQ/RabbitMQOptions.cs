namespace Pokok.BuildingBlocks.Messaging.RabbitMQ
{
    public class RabbitMQOptions
    {
        public string HostName { get; set; } = default!;
        public int Port { get; set; } = 5672;
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
    }
}
