using Pokok.BuildingBlocks.Persistence.Entities;
using Pokok.BuildingBlocks.Persistence.Specifications.Core;

namespace Pokok.BuildingBlocks.Persistence.Specifications.Examples
{
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

    public class User: EntityBase
    {
    }
}
