using MyAppRoot.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace MyAppRoot.AppServices.StaffServices;

public interface IStaffAppService : IDisposable
{
    Task<StaffViewDto?> FindAsync(Guid id);
    public Task<List<StaffViewDto>> GetListAsync(StaffSearchDto filter);
    public Task<IList<string>> GetRolesAsync(Guid id);
    public Task<IList<AppRole>> GetAppRolesAsync(Guid id);
    public Task<IdentityResult> UpdateRolesAsync(Guid id, Dictionary<string, bool> roles);
    Task<IdentityResult> UpdateAsync(StaffUpdateDto resource);
}
