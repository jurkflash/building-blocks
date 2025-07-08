namespace Pokok.BuildingBlocks.Domain.ValueObjects
{
    public sealed class TenantId : SingleValueObject<Guid>
    {
        public TenantId(Guid value) : base(value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("TenantId cannot be empty.", nameof(value));
        }

        public static TenantId New() => new(Guid.NewGuid());
    }
}
