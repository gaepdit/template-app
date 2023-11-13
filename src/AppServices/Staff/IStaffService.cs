using GaEpd.AppLibrary.Pagination;
using Microsoft.AspNetCore.Identity;
using MyApp.AppServices.Staff.Dto;
using MyApp.Domain.Identity;

namespace MyApp.AppServices.Staff;

public interface IStaffService : IDisposable, IAsyncDisposable
{
    Task<StaffViewDto> GetCurrentUserAsync();
    Task<StaffViewDto?> FindAsync(string id);
    Task<List<StaffViewDto>> GetListAsync(StaffSearchDto spec);
    Task<IPaginatedResult<StaffSearchResultDto>> SearchAsync(StaffSearchDto spec, PaginatedRequest paging);
    Task<IList<string>> GetRolesAsync(string id);
    Task<IList<AppRole>> GetAppRolesAsync(string id);
    Task<IdentityResult> UpdateRolesAsync(string id, Dictionary<string, bool> roles);
    Task<IdentityResult> UpdateAsync(string id, StaffUpdateDto resource);
}
