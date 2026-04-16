namespace Pokok.BuildingBlocks.MultiTenancy
{
    /// <summary>
    /// Provides the current tenant context for the executing request or operation.
    /// Returns <c>null</c> for system-level operations with no tenant scope.
    /// Implement this interface to extract tenant identity from your request pipeline (JWT claim, HTTP header, subdomain, etc.).
    /// </summary>
    public interface ITenantProvider
    {
        /// <summary>
        /// Gets the tenant identifier for the current execution context.
        /// </summary>
        /// <returns>The current <see cref="TenantId"/>, or <c>null</c> if no tenant scope is active.</returns>
        TenantId? GetCurrentTenantId();
    }
}
