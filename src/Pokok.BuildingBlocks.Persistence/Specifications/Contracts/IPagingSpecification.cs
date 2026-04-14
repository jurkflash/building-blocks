namespace Pokok.BuildingBlocks.Persistence.Specifications.Contracts
{
    /// <summary>
    /// Specification contract for pagination, defining skip/take and an enabled flag.
    /// </summary>
    public interface IPagingSpecification : ISpecification<object> // Will be cast properly in handlers
    {
        int Skip { get; }
        int Take { get; }
        bool IsPagingEnabled { get; }
    }
}
