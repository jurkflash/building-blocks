namespace Pokok.BuildingBlocks.Domain.Abstractions
{
    public abstract record EntityId<T>(T Value)
    {
        public override string ToString() => Value?.ToString() ?? string.Empty;
    }
}
