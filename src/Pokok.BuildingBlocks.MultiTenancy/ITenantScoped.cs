namespace Pokok.BuildingBlocks.MultiTenancy
{
    /// <summary>
    /// Marker interface for entities that belong to a specific tenant.
    /// The <see cref="TenantId"/> is immutable after construction — entities cannot be moved between tenants.
    /// </summary>
    public interface ITenantScoped
    {
        TenantId TenantId { get; }
    }
}
