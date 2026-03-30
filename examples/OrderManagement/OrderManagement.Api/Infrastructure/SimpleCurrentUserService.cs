using Pokok.BuildingBlocks.Common;

namespace OrderManagement.Api.Infrastructure;

/// <summary>
/// Simple implementation of ICurrentUserService for demo purposes
/// In a real application, this would integrate with your authentication system
/// </summary>
public class SimpleCurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SimpleCurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId
    {
        get
        {
            // In a real app, you would get this from the authenticated user's claims
            // For demo purposes, return a default user
            var userId = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
            return userId ?? "demo-user";
        }
    }
}
