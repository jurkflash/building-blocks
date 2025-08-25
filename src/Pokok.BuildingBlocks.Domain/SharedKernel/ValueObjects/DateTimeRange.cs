using Pokok.BuildingBlocks.Domain.Abstractions;
using Pokok.BuildingBlocks.Domain.Exceptions;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects
{
    public sealed class DateTimeRange : ValueObject
    {
        public DateTimeOffset Start { get; }
        public DateTimeOffset End { get; }

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

        public bool Contains(DateTimeOffset value) => value >= Start && value <= End;

        public bool Overlaps(DateTimeRange other) =>
            Start < other.End && End > other.Start;

        public TimeSpan Duration => End - Start;

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Start;
            yield return End;
        }

        public override string ToString() => $"{Start:u} to {End:u}";
    }
}
