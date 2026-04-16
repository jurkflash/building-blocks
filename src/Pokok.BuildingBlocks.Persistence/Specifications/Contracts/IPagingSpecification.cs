namespace Pokok.BuildingBlocks.Persistence.Specifications.Contracts
{
    /// <summary>
    /// Specification contract for pagination, defining skip/take and an enabled flag.
    /// </summary>
    public interface IPagingSpecification : ISpecification<object> // Will be cast properly in handlers
    {
        /// <summary>
        /// Gets the number of items to skip.
        /// </summary>
        int Skip { get; }

        /// <summary>
        /// Gets the number of items to take.
        /// </summary>
        int Take { get; }

        /// <summary>
        /// Gets a value indicating whether paging is enabled.
        /// </summary>
        bool IsPagingEnabled { get; }
    }
}
