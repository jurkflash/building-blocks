namespace Pokok.BuildingBlocks.MultiTenancy
{
    public abstract class TenantEntity : Entity, ITenantScoped
    {
        public TenantId TenantId { get; protected set; }

        protected TenantEntity(TenantId tenantId)
        {
            TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        }
    }
}
