using MyAppRoot.Domain.Entities;

namespace MyAppRoot.AppServices.UserServices;

public interface IUserService
{
    public Task<ApplicationUser?> GetCurrentUserAsync();
}
