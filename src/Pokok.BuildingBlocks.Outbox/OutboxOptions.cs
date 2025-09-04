namespace Pokok.BuildingBlocks.Outbox
{
    public class OutboxOptions
    {
        public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(10);
    }
}
