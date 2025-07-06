namespace Pokok.BuildingBlocks.MultiTenancy
{
    public interface ITenantProvider
    {
        TenantId? GetCurrentTenantId();
    }
}
