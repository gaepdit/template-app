using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using MyApp.Domain.Identity;

namespace MyApp.AppServices.UserServices;

public class UserService : IUserService
{
    internal const double UserExpirationMinutes = 30.0;
    
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMemoryCache _cache;

    public UserService(
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContextAccessor,
        IMemoryCache cache)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _cache = cache;
    }

    public async Task<ApplicationUser?> GetCurrentUserAsync()
    {
        var principal = _httpContextAccessor.HttpContext?.User;
        return principal is null ? null : await _userManager.GetUserAsync(principal);
    }

    /// <summary>
    /// Asynchronously retrieves User object based on it's unique identifier.
    /// </summary>
    /// <param name="id">User's unique identifier.</param>
    /// <returns>The User associated with the provided id if present and null otherwise.</returns>
    public async Task<ApplicationUser?> FindUserAsync(string id)
    {
        var user = _cache.Get<ApplicationUser>(id);
        if (user is not null) return user;
        
        user = await _userManager.FindByIdAsync(id);
        if(user is not null) _cache.Set(id, user, TimeSpan.FromMinutes(UserExpirationMinutes));
        return user;
    }
}
