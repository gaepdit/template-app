using GaEpd.AppLibrary.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using MyApp.Domain.Identity;
using MyApp.TestData.Identity;

namespace MyApp.LocalRepository.Identity;

public sealed class LocalUserStore :
    IUserRoleStore<ApplicationUser>, // inherits IUserStore<ApplicationUser>
    IUserLoginStore<ApplicationUser>,
    IQueryableUserStore<ApplicationUser>
{
    public IQueryable<ApplicationUser> Users => UserStore.AsQueryable();
    internal ICollection<ApplicationUser> UserStore { get; } = UserData.GetUsers.ToList();
    internal ICollection<IdentityRole> Roles { get; } = UserData.GetRoles.ToList();
    private List<IdentityUserRole<string>> UserRoles { get; } = [];
    private List<UserLogin> UserLogins { get; } = [];

    // IUserStore
    public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken) =>
        Task.FromResult(user.Id);

    public Task<string?> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken) =>
        Task.FromResult(user.UserName);

    public Task SetUserNameAsync(ApplicationUser user, string? userName, CancellationToken cancellationToken)
    {
        user.UserName = userName;
        return Task.CompletedTask;
    }

    public Task<string?> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken) =>
        Task.FromResult(user.NormalizedUserName);

    public Task SetNormalizedUserNameAsync(ApplicationUser user, string? normalizedName,
        CancellationToken cancellationToken)
    {
        user.NormalizedUserName = normalizedName;
        return Task.CompletedTask;
    }

    public Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        UserStore.Add(user);
        return Task.FromResult(IdentityResult.Success);
    }

    public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var existingUser = await FindByIdAsync(user.Id, cancellationToken).ConfigureAwait(false)
                           ?? throw new EntityNotFoundException<ApplicationUser>(user.Id);
        UserStore.Remove(existingUser);
        UserStore.Add(user);
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var existingUser = await FindByIdAsync(user.Id, cancellationToken).ConfigureAwait(false);
        if (existingUser is not null) UserStore.Remove(existingUser);
        return IdentityResult.Success;
    }

    public Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken cancellationToken) =>
        Task.FromResult(UserStore.SingleOrDefault(u => u.Id == userId));

    public Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) =>
        Task.FromResult(UserStore.SingleOrDefault(u =>
            string.Equals(u.NormalizedUserName, normalizedUserName, StringComparison.InvariantCultureIgnoreCase)));

    // IUserRoleStore
    public Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
    {
        var roleId = Roles.SingleOrDefault(r =>
            string.Equals(r.Name, roleName, StringComparison.InvariantCultureIgnoreCase))?.Id;
        if (roleId is null) return Task.CompletedTask;

        var exists = UserRoles.Exists(e => e.UserId == user.Id && e.RoleId == roleId);
        if (!exists) UserRoles.Add(new IdentityUserRole<string> { RoleId = roleId, UserId = user.Id });

        return Task.CompletedTask;
    }

    public Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
    {
        var roleId = Roles.SingleOrDefault(r =>
            string.Equals(r.Name, roleName, StringComparison.InvariantCultureIgnoreCase))?.Id;
        if (roleId is null) return Task.CompletedTask;

        var userRole = UserRoles.SingleOrDefault(e => e.UserId == user.Id && e.RoleId == roleId);
        if (userRole is not null) UserRoles.Remove(userRole);

        return Task.CompletedTask;
    }

    public Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var roleIdsForUser = UserRoles
            .Where(e => e.UserId == user.Id)
            .Select(e => e.RoleId);
        var rolesForUser = Roles
            .Where(r => roleIdsForUser.Contains(r.Id))
            .Select(r => r.Name ?? "").ToList();
        return Task.FromResult<IList<string>>(rolesForUser);
    }

    public Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
    {
        var roleId = Roles.SingleOrDefault(r =>
            string.Equals(r.Name, roleName, StringComparison.InvariantCultureIgnoreCase))?.Id;
        return Task.FromResult(UserRoles.Exists(e => e.UserId == user.Id && e.RoleId == roleId));
    }

    public Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        var roleId = Roles.SingleOrDefault(r =>
            string.Equals(r.Name, roleName, StringComparison.InvariantCultureIgnoreCase))?.Id;
        var userIdsInRole = UserRoles
            .Where(e => e.RoleId == roleId)
            .Select(e => e.UserId);
        var usersInRole = UserStore
            .Where(u => userIdsInRole.Contains(u.Id)).ToList();
        return Task.FromResult<IList<ApplicationUser>>(usersInRole);
    }

    public void Dispose()
    {
        // Method intentionally left empty.
    }

    public Task AddLoginAsync(ApplicationUser user, UserLoginInfo login, CancellationToken cancellationToken)
    {
        UserLogins.Add(new UserLogin
        {
            LoginProvider = login.LoginProvider,
            ProviderDisplayName = login.ProviderDisplayName,
            ProviderKey = login.ProviderKey,
            UserId = user.Id,
        });
        return Task.CompletedTask;
    }

    public Task RemoveLoginAsync(ApplicationUser user, string loginProvider, string providerKey,
        CancellationToken cancellationToken)
    {
        var ul = UserLogins.SingleOrDefault(ul =>
            ul.UserId == user.Id && ul.LoginProvider == loginProvider && ul.ProviderKey == providerKey);
        if (ul is not null) UserLogins.Remove(ul);
        return Task.CompletedTask;
    }

    public Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user, CancellationToken cancellationToken) =>
        Task.FromResult<IList<UserLoginInfo>>(
            UserLogins.Where(ul => ul.UserId == user.Id)
                .Select(ul => new UserLoginInfo(ul.LoginProvider, ul.ProviderKey, ul.ProviderDisplayName))
                .ToList());

    public Task<ApplicationUser?> FindByLoginAsync(string loginProvider, string providerKey,
        CancellationToken cancellationToken)
    {
        var userId = UserLogins
            .SingleOrDefault(ul => ul.LoginProvider == loginProvider && ul.ProviderKey == providerKey)?.UserId;
        return Task.FromResult(UserStore.SingleOrDefault(user => user.Id == userId));
    }

    private sealed class UserLogin
    {
        public string LoginProvider { get; init; } = string.Empty;
        public string ProviderKey { get; init; } = string.Empty;
        public string? ProviderDisplayName { get; init; }
        public string UserId { get; init; } = string.Empty;
    }
}
