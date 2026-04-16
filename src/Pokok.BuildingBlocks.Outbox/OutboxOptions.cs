namespace Pokok.BuildingBlocks.Outbox
{
    /// <summary>
    /// Configuration options for the outbox processor.
    /// Controls the polling interval between processing cycles (default: 10 seconds).
    /// </summary>
    public class OutboxOptions
    {
        /// <summary>
        /// Gets or sets the polling interval between outbox processing cycles. Defaults to 10 seconds.
        /// </summary>
        public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(10);
    }
}
