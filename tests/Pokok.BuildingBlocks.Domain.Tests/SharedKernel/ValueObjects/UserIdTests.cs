using Pokok.BuildingBlocks.Domain.Exceptions;
using Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects;
using Xunit;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects;

public class UserIdTests
{
    [Fact]
    public void Constructor_WithValidGuid_CreatesUserId()
    {
        var guid = Guid.NewGuid();

        var userId = new UserId(guid);

        Assert.Equal(guid, userId.Value);
    }

    [Fact]
    public void Constructor_WithEmptyGuid_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() => new UserId(Guid.Empty));

        Assert.Equal("UserId cannot be empty", exception.Message);
    }

    [Fact]
    public void New_CreatesUserIdWithNonEmptyGuid()
    {
        var userId = UserId.New();

        Assert.NotEqual(Guid.Empty, userId.Value);
    }

    [Fact]
    public void Equals_TwoUserIdsWithSameGuid_ReturnsTrue()
    {
        var guid = Guid.NewGuid();
        var userId1 = new UserId(guid);
        var userId2 = new UserId(guid);

        Assert.Equal(userId1, userId2);
    }

    [Fact]
    public void Equals_TwoUserIdsWithDifferentGuids_ReturnsFalse()
    {
        var userId1 = UserId.New();
        var userId2 = UserId.New();

        Assert.NotEqual(userId1, userId2);
    }

    [Fact]
    public void ToString_WithValidGuid_ReturnsGuidString()
    {
        var guid = Guid.NewGuid();
        var userId = new UserId(guid);

        Assert.Equal(guid.ToString(), userId.ToString());
    }
}
