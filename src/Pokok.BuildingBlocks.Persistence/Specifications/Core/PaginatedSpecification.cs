using System.Linq.Expressions;

namespace Pokok.BuildingBlocks.Persistence.Specifications.Core
{
    /// <summary>
    /// Base specification that adds pagination support (page number and page size) to a filter criteria.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public abstract class PaginatedSpecification<T> : BaseSpecification<T>
    {
        public int PageNumber { get; }
        public int PageSize { get; }

        protected PaginatedSpecification(Expression<Func<T, bool>> criteria, int pageNumber, int pageSize)
        : base(criteria)
        {
            if (pageNumber <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be greater than zero.");

            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");

            PageNumber = pageNumber;
            PageSize = pageSize;

            var skip = (PageNumber - 1) * PageSize;
            ApplyPaging(skip, PageSize);
        }
    }
}
