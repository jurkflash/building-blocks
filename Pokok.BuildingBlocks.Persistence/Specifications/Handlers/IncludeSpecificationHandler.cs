using Microsoft.EntityFrameworkCore;
using Pokok.BuildingBlocks.Persistence.Specifications.Contracts;

namespace Pokok.BuildingBlocks.Persistence.Specifications.Handlers
{
    public class IncludeSpecificationHandler<T> : ISpecificationHandler<T> where T : class
    {
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
