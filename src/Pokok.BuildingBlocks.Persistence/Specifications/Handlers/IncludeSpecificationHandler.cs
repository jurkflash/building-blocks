using Microsoft.EntityFrameworkCore;
using Pokok.BuildingBlocks.Persistence.Specifications.Contracts;

namespace Pokok.BuildingBlocks.Persistence.Specifications.Handlers
{
    /// <summary>
    /// Applies a specification's include expressions to an <see cref="IQueryable{T}"/> for eager loading.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public class IncludeSpecificationHandler<T> : ISpecificationHandler<T> where T : class
    {
        /// <inheritdoc />
        public IQueryable<T> Apply(IQueryable<T> query, ISpecification<T> specification)
        {
            if (specification is IIncludeSpecification<T> includeSpec)
            {
                foreach (var include in includeSpec.Includes)
                {
                    query = query.Include(include);
                }
            }
            return query;
        }
    }
}
