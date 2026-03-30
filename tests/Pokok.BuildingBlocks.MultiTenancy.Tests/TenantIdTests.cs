using NSubstitute;
using Xunit;

namespace Pokok.BuildingBlocks.MultiTenancy;

public class TenantIdTests
{
    [Fact]
    public void Constructor_WithValidValue_CreatesTenantId()
    {
        var tenantId = new TenantId("tenant-abc");

        Assert.Equal("tenant-abc", tenantId.Value);
    }

    [Fact]
    public void Constructor_WithNullValue_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new TenantId(null!));
    }

    [Fact]
    public void Constructor_WithEmptyValue_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new TenantId(""));
    }

    [Fact]
    public void Constructor_WithWhitespaceValue_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new TenantId("   "));
    }

    [Fact]
    public void Equals_TwoTenantIdsWithSameValue_ReturnsTrue()
    {
        var tenantId1 = new TenantId("tenant-abc");
        var tenantId2 = new TenantId("tenant-abc");

        Assert.Equal(tenantId1, tenantId2);
    }

    [Fact]
    public void Equals_TwoTenantIdsWithDifferentValues_ReturnsFalse()
    {
        var tenantId1 = new TenantId("tenant-abc");
        var tenantId2 = new TenantId("tenant-xyz");

        Assert.NotEqual(tenantId1, tenantId2);
    }

    [Fact]
    public void ToString_WithValidValue_ReturnsValue()
    {
        var tenantId = new TenantId("tenant-abc");

        Assert.Equal("tenant-abc", tenantId.ToString());
    }

    [Fact]
    public void ITenantProvider_GetCurrentTenantId_WhenTenantSet_ReturnsTenantId()
    {
        var provider = Substitute.For<ITenantProvider>();
        var tenantId = new TenantId("tenant-abc");
        provider.GetCurrentTenantId().Returns(tenantId);

        var result = provider.GetCurrentTenantId();

        Assert.Equal(tenantId, result);
    }

    [Fact]
    public void ITenantProvider_GetCurrentTenantId_WhenNoTenant_ReturnsNull()
    {
        var provider = Substitute.For<ITenantProvider>();
        provider.GetCurrentTenantId().Returns((TenantId?)null);

        var result = provider.GetCurrentTenantId();

        Assert.Null(result);
    }
}
