namespace Pokok.BuildingBlocks.Persistence.Specifications.Contracts
{
    public interface IPagingSpecification : ISpecification<object> // Will be cast properly in handlers
    {
        int Skip { get; }
        int Take { get; }
        bool IsPagingEnabled { get; }
    }
}
