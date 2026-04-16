namespace Pokok.BuildingBlocks.Messaging.RabbitMQ
{
    /// <summary>
    /// Configuration options for connecting to a RabbitMQ broker.
    /// Bind to an <c>IOptions&lt;RabbitMQOptions&gt;</c> from application configuration.
    /// </summary>
    public class RabbitMQOptions
    {
        /// <summary>
        /// Gets or sets the RabbitMQ server hostname.
        /// </summary>
        public string HostName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the RabbitMQ server port. Defaults to 5672.
        /// </summary>
        public int Port { get; set; } = 5672;

        /// <summary>
        /// Gets or sets the username for authentication. Defaults to <c>guest</c>.
        /// </summary>
        public string UserName { get; set; } = "guest";

        /// <summary>
        /// Gets or sets the password for authentication. Defaults to <c>guest</c>.
        /// </summary>
        public string Password { get; set; } = "guest";
    }
}
