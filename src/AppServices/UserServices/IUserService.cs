using MyApp.Domain.Identity;

namespace MyApp.AppServices.UserServices;

public interface IUserService
{
    public Task<ApplicationUser?> GetCurrentUserAsync();
    public Task<ApplicationUser?> FindUserAsync(string id);
}
