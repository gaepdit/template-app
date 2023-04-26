using Microsoft.AspNetCore.Identity;
using MyAppRoot.Domain.Identity;

namespace MyAppRoot.AppServices.Staff;

public interface IStaffAppService : IDisposable
{
    Task<StaffViewDto> GetCurrentUserAsync();
    Task<StaffViewDto?> FindAsync(string id);
    Task<List<StaffViewDto>> GetListAsync(StaffSearchDto filter);
    Task<IList<string>> GetRolesAsync(string id);
    Task<IList<AppRole>> GetAppRolesAsync(string id);
    Task<IdentityResult> UpdateRolesAsync(string id, Dictionary<string, bool> roles);
    Task<IdentityResult> UpdateAsync(StaffUpdateDto resource);
}
