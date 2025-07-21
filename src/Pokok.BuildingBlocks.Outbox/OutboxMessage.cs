using Pokok.BuildingBlocks.Domain.SharedKernel.Enums;

namespace Pokok.BuildingBlocks.Outbox
{
    public class OutboxMessage
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// When the event was originally raised by the domain
        /// </summary>
        public DateTime OccurredOnUtc { get; private set; } = DateTime.UtcNow;

        /// <summary>
        /// Backing field for EF Core to store the string representation
        /// </summary>
        private string _typeValue = default!;

        /// <summary>
        /// The type of the message (used by consumers to deserialize or dispatch)
        /// </summary>
        public OutboxMessageType Type
        {
            get => OutboxMessageType.From(_typeValue);
            private set => _typeValue = value.Value;
        }
        /// <summary>
        /// Serialized event or command payload (usually as JSON)
        /// </summary>
        public string Payload { get; private set; }

        /// <summary>
        /// Identifier for which app published this message (e.g., "identityserver", "jmb.portal")
        /// </summary>
        public string SourceApp { get; private set; }

        /// <summary>
        /// When the message was successfully processed
        /// </summary>
        public DateTime? ProcessedOnUtc { get; private set; }

        /// <summary>
        /// Error message if processing failed
        /// </summary>
        public string? Error { get; private set; }

        // EF Core constructor
        private OutboxMessage() { }

        public OutboxMessage(
            OutboxMessageType type,
            string payload,
            string sourceApp)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Payload = payload ?? throw new ArgumentNullException(nameof(payload));
            SourceApp = sourceApp ?? throw new ArgumentNullException(nameof(sourceApp));
        }

        public void MarkAsProcessed()
        {
            ProcessedOnUtc = DateTime.UtcNow;
            Error = null;
        }

        public void MarkAsFailed(string error)
        {
            ProcessedOnUtc = null;
            Error = error;
        }
    }
}
