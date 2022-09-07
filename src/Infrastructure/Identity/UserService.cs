using MyAppRoot.AppServices.UserServices;
using MyAppRoot.Domain.Entities;
using MyAppRoot.Infrastructure.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace MyAppRoot.Infrastructure.Identity;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(
        UserManager<ApplicationUser> userManager,
        AppDbContext context,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ApplicationUser?> GetCurrentUserAsync() => throw new NotImplementedException();

    public async Task<IList<string>> GetCurrentUserRolesAsync() => throw new NotImplementedException();

    public async Task<ApplicationUser?> FindUserByIdAsync(string userId) => throw new NotImplementedException();

    public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user) => throw new NotImplementedException();

    public async Task<IList<string>> GetUserRolesAsync(Guid userId) => throw new NotImplementedException();

    public async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName) =>
        throw new NotImplementedException();

    public async Task<bool> IsInRoleAsync(ApplicationUser user, string role) => throw new NotImplementedException();

    public async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role) =>
        throw new NotImplementedException();

    public async Task<IdentityResult> RemoveFromRoleAsync(ApplicationUser user, string role) =>
        throw new NotImplementedException();
}
