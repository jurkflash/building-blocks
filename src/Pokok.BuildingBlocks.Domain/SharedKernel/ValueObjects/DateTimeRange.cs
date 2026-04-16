using Pokok.BuildingBlocks.Domain.Abstractions;
using Pokok.BuildingBlocks.Domain.Exceptions;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects
{
    /// <summary>
    /// Immutable value object representing a date/time range with a start and end.
    /// Throws <see cref="DomainException"/> if <see cref="End"/> is before <see cref="Start"/>.
    /// Supports overlap detection, containment checks, and duration calculation.
    /// </summary>
    public sealed class DateTimeRange : ValueObject
    {
        /// <summary>Gets the start of the range.</summary>
        public DateTimeOffset Start { get; }

        /// <summary>Gets the end of the range.</summary>
        public DateTimeOffset End { get; }

        /// <summary>
        /// Initializes a new <see cref="DateTimeRange"/> with the specified start and end.
        /// </summary>
        /// <param name="start">The start of the range.</param>
        /// <param name="end">The end of the range.</param>
        public DateTimeRange(DateTimeOffset start, DateTimeOffset end)
        {
            Start = start;
            End = end;

            Validate();
        }

        protected override void Validate()
        {
            if (End < Start)
                throw new DomainException("End must be greater than or equal to start.");
        }

        /// <summary>
        /// Determines whether the specified value falls within this range (inclusive).
        /// </summary>
        /// <param name="value">The date/time value to check.</param>
        /// <returns><c>true</c> if the value is within the range; otherwise, <c>false</c>.</returns>
        public bool Contains(DateTimeOffset value) => value >= Start && value <= End;

        /// <summary>
        /// Determines whether this range overlaps with another range.
        /// </summary>
        /// <param name="other">The other date/time range to check against.</param>
        /// <returns><c>true</c> if the ranges overlap; otherwise, <c>false</c>.</returns>
        public bool Overlaps(DateTimeRange other) =>
            Start < other.End && End > other.Start;

        /// <summary>Gets the duration of the range.</summary>
        public TimeSpan Duration => End - Start;

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Start;
            yield return End;
        }

        public override string ToString() => $"{Start:u} to {End:u}";
    }
}
