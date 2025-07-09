namespace Pokok.BuildingBlocks.Domain.Abstractions
{
    public abstract class ValueObject : IEquatable<ValueObject>
    {
        protected abstract IEnumerable<object?> GetEqualityComponents();

        public override bool Equals(object? obj)
        {
            return Equals(obj as ValueObject);
        }

        public bool Equals(ValueObject? other)
        {
            if (other is null || GetType() != other.GetType())
                return false;

            return GetEqualityComponents()
                .SequenceEqual(other.GetEqualityComponents());
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                foreach (var obj in GetEqualityComponents())
                    hash = hash * 23 + (obj?.GetHashCode() ?? 0);

                return hash;
            }
        }

        public static bool operator ==(ValueObject? left, ValueObject? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ValueObject? left, ValueObject? right)
        {
            return !Equals(left, right);
        }
    }
}
