using Pokok.BuildingBlocks.Persistence.Entities;
using Pokok.BuildingBlocks.Persistence.Specifications.Core;

namespace Pokok.BuildingBlocks.Persistence.Specifications.Examples
{
    /// <summary>
    /// Example specification that filters for active (non-deleted) users.
    /// Demonstrates how to extend <see cref="BaseSpecification{T}"/> with a concrete criteria expression.
    /// </summary>
    public class ActiveUsersSpecification : BaseSpecification<User>
    {
        public ActiveUsersSpecification()
        : base(user => /*user.IsActive &&*/ !user.IsDeleted)
        {
        }

        /*
         var spec = new ActiveUsersSpecification();

        var users = await SpecificationEvaluator
            .GetQuery(context.Users, spec)
            .ToListAsync();
          
         */

    }

    /// <summary>
    /// Example entity used by specification samples. Not intended for production use.
    /// </summary>
    public class User: EntityBase
    {
    }
}
