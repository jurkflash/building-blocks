namespace Pokok.BuildingBlocks.Domain.ValueObjects
{
    public sealed class DateTimeRange : ValueObject
    {
        public DateTimeOffset Start { get; }
        public DateTimeOffset End { get; }

        public DateTimeRange(DateTimeOffset start, DateTimeOffset end)
        {
            if (end < start)
                throw new ArgumentException("End must be greater than or equal to start.");

            Start = start;
            End = end;
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
