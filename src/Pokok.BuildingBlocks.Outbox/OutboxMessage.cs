using Pokok.BuildingBlocks.Domain.SharedKernel.Enums;

namespace Pokok.BuildingBlocks.Outbox
{
    /// <summary>
    /// Represents a message in the transactional outbox.
    /// Tracks type, serialized payload, source application, processing status, and error state.
    /// State machine: New → Processed (on success) or Failed (on error, remains eligible for retry).
    /// </summary>
    public class OutboxMessage
    {
        /// <summary>
        /// Gets the unique identifier for the outbox message.
        /// </summary>
        public Guid Id { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// When the event was originally raised by the domain
        /// </summary>
        public DateTime OccurredOnUtc { get; private set; }

        /// <summary>
        /// The type of the message (used by consumers to deserialize or dispatch)
        /// </summary>
        public OutboxMessageType Type { get; private set; }
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

        /// <summary>
        /// Initializes a new instance of <see cref="OutboxMessage"/> with the specified type, payload, and source.
        /// </summary>
        /// <param name="type">The message type identifier.</param>
        /// <param name="payload">The serialized message payload.</param>
        /// <param name="sourceApp">The identifier of the application that created this message.</param>
        /// <param name="occurredOnUtc">The UTC timestamp when the event occurred. Defaults to <see cref="DateTime.UtcNow"/>.</param>
        public OutboxMessage(
            OutboxMessageType type,
            string payload,
            string sourceApp,
            DateTime? occurredOnUtc = null)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Payload = payload ?? throw new ArgumentNullException(nameof(payload));
            SourceApp = sourceApp ?? throw new ArgumentNullException(nameof(sourceApp));
            OccurredOnUtc = occurredOnUtc ?? DateTime.UtcNow;
        }

        /// <summary>
        /// Marks this message as successfully processed and clears any previous error.
        /// </summary>
        public void MarkAsProcessed()
        {
            ProcessedOnUtc = DateTime.UtcNow;
            Error = null;
        }

        /// <summary>
        /// Marks this message as failed with the specified error, making it eligible for retry.
        /// </summary>
        /// <param name="error">The error message describing the failure.</param>
        public void MarkAsFailed(string error)
        {
            ProcessedOnUtc = null;
            Error = error;
        }
    }
}
