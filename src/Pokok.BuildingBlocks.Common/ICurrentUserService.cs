namespace Pokok.BuildingBlocks.Common
{
    /// <summary>
    /// Provides access to the current authenticated user's identity.
    /// Used by persistence and audit modules to populate created-by / modified-by fields.
    /// Returns <c>null</c> when no user is authenticated or the context is unavailable.
    /// </summary>
    public interface ICurrentUserService
    {
        /// <summary>
        /// Gets the current user's identifier, or <c>null</c> if unauthenticated.
        /// </summary>
        string? UserId { get; }
    }
}
