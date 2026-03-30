using NSubstitute;
using Xunit;

namespace Pokok.BuildingBlocks.Common;

public class ICurrentUserServiceTests
{
    [Fact]
    public void UserId_WhenUserIsAuthenticated_ReturnsUserId()
    {
        var service = Substitute.For<ICurrentUserService>();
        service.UserId.Returns("user-123");

        var result = service.UserId;

        Assert.Equal("user-123", result);
    }

    [Fact]
    public void UserId_WhenUserIsAnonymous_ReturnsNull()
    {
        var service = Substitute.For<ICurrentUserService>();
        service.UserId.Returns((string?)null);

        var result = service.UserId;

        Assert.Null(result);
    }
}
