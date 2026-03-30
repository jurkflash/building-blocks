namespace Pokok.BuildingBlocks.Messaging.RabbitMQ
{
    public class RabbitMQOptions
    {
        public string HostName { get; set; } = default!;
        public int Port { get; set; } = 5672;
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string VirtualHost { get; set; } = "/";

        /// <summary>
        /// The bounded capacity of the in-process <see cref="System.Threading.Channels.Channel{T}"/>
        /// used to decouple message receipt from message processing in consumers.
        /// </summary>
        public int ConsumerChannelCapacity { get; set; } = 256;
    }
}
