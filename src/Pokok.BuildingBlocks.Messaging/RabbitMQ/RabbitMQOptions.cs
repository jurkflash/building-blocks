namespace Pokok.BuildingBlocks.Messaging.RabbitMQ
{
    /// <summary>
    /// Configuration options for connecting to a RabbitMQ broker.
    /// Bind to an <c>IOptions&lt;RabbitMQOptions&gt;</c> from application configuration.
    /// </summary>
    public class RabbitMQOptions
    {
        public string HostName { get; set; } = default!;
        public int Port { get; set; } = 5672;
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
    }
}
