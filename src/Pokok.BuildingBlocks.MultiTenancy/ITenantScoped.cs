namespace Pokok.BuildingBlocks.MultiTenancy
{
    public interface ITenantScoped
    {
        TenantId TenantId { get; }
    }
}
