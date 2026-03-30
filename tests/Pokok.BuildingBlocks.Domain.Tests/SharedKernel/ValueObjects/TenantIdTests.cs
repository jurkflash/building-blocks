using Pokok.BuildingBlocks.Domain.Exceptions;
using Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects;
using Xunit;

namespace Pokok.BuildingBlocks.Domain.SharedKernel.ValueObjects;

public class TenantIdTests
{
    [Fact]
    public void Constructor_WithValidGuid_CreatesTenantId()
    {
        var guid = Guid.NewGuid();

        var tenantId = new TenantId(guid);

        Assert.Equal(guid, tenantId.Value);
    }

    [Fact]
    public void Constructor_WithEmptyGuid_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() => new TenantId(Guid.Empty));

        Assert.Equal("TenantId cannot be empty.", exception.Message);
    }

    [Fact]
    public void New_CreatesTenantIdWithNonEmptyGuid()
    {
        var tenantId = TenantId.New();

        Assert.NotEqual(Guid.Empty, tenantId.Value);
    }

    [Fact]
    public void Equals_TwoTenantIdsWithSameGuid_ReturnsTrue()
    {
        var guid = Guid.NewGuid();
        var tenantId1 = new TenantId(guid);
        var tenantId2 = new TenantId(guid);

        Assert.Equal(tenantId1, tenantId2);
    }

    [Fact]
    public void Equals_TwoTenantIdsWithDifferentGuids_ReturnsFalse()
    {
        var tenantId1 = TenantId.New();
        var tenantId2 = TenantId.New();

        Assert.NotEqual(tenantId1, tenantId2);
    }
}
