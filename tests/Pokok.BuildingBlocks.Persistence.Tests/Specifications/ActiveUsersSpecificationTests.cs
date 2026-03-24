using Microsoft.EntityFrameworkCore;
using Pokok.BuildingBlocks.Persistence.Specifications.Core;
using Pokok.BuildingBlocks.Persistence.Specifications.Examples;
using Pokok.BuildingBlocks.Persistence.Specifications.Handlers;
using Xunit;

namespace Pokok.BuildingBlocks.Persistence.Specifications;

public class ActiveUsersSpecificationTests
{
    [Fact]
    public void Constructor_WithActiveUsersSpec_FiltersDeletedUsers()
    {
        var spec = new ActiveUsersSpecification();

        Assert.NotNull(spec.Criteria);

        var activeUser = new User { IsDeleted = false };
        var deletedUser = new User { IsDeleted = true };

        var predicate = spec.ToPredicate();

        Assert.True(predicate(activeUser));
        Assert.False(predicate(deletedUser));
    }
}
