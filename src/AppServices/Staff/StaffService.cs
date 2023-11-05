using AutoMapper;
using GaEpd.AppLibrary.Domain.Repositories;
using GaEpd.AppLibrary.Pagination;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using MyApp.AppServices.Staff.Dto;
using MyApp.AppServices.UserServices;
using MyApp.Domain.Entities.Offices;
using MyApp.Domain.Identity;

namespace MyApp.AppServices.Staff;

public sealed class StaffService : IStaffService
{
    private const double UserExpirationMinutes = UserService.UserExpirationMinutes;
    
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOfficeRepository _officeRepository;
    private readonly IUserService _userService;
    private readonly IMemoryCache _cache;
    private readonly IMapper _mapper;

    public StaffService(
        IUserService userService,
        UserManager<ApplicationUser> userManager,
        IMapper mapper,
        IOfficeRepository officeRepository,
        IMemoryCache cache)
    {
        _userService = userService;
        _userManager = userManager;
        _mapper = mapper;
        _officeRepository = officeRepository;
        _cache = cache;
    }

    public async Task<StaffViewDto> GetCurrentUserAsync()
    {
        var user = await _userService.GetCurrentUserAsync()
            ?? throw new CurrentUserNotFoundException();
        return _mapper.Map<StaffViewDto>(user);
    }

    /// <summary>
    /// Asynchronously retrieves <see cref="ApplicationUser"/> based on its unique identifier.
    /// Private method, used only within the service, same as FindUserAsync(string) at <see cref="IUserService"/>
    /// </summary>
    /// <param name="id">User's unique identifier.</param>
    /// <returns>The User whose associated with the id provided if present and null otherwise.</returns>
    private async Task<ApplicationUser?> GetUserCachedAsync(string id) => await _userService.FindUserAsync(id);
    
    /// <summary>
    /// Asynchronously retrieves <see cref="ApplicationUser"/> as <see cref="StaffViewDto"/> based on it's unique
    /// identifier.
    /// </summary>
    /// <param name="id">Staff member's unique identifier within the system.</param>
    /// <returns> The associated StaffViewDto with the provided identifier if presented and null otherwise. </returns>
    public async Task<StaffViewDto?> FindAsync(string id)
    {
        var user = await GetUserCachedAsync(id);
        return _mapper.Map<StaffViewDto?>(user);
    }

    public async Task<List<StaffViewDto>> GetListAsync(StaffSearchDto spec)
    {
        var users = string.IsNullOrEmpty(spec.Role)
            ? _userManager.Users.ApplyFilter(spec)
            : (await _userManager.GetUsersInRoleAsync(spec.Role)).AsQueryable().ApplyFilter(spec);

        return _mapper.Map<List<StaffViewDto>>(users);
    }

    public async Task<IPaginatedResult<StaffSearchResultDto>> SearchAsync(StaffSearchDto spec, PaginatedRequest paging)
    {
        var users = string.IsNullOrEmpty(spec.Role)
            ? _userManager.Users.ApplyFilter(spec)
            : (await _userManager.GetUsersInRoleAsync(spec.Role)).AsQueryable().ApplyFilter(spec);
        var list = users.Skip(paging.Skip).Take(paging.Take);
        var listMapped = _mapper.Map<List<StaffSearchResultDto>>(list);

        return new PaginatedResult<StaffSearchResultDto>(listMapped, users.Count(), paging);
    }

    /// <summary>
    /// Asynchronously retrieves a list of roles associated with a staff member by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the staff member.</param>
    /// <returns>
    /// A list of role names to which the staff member belongs. An empty list is returned if the member is not found.
    /// </returns>
    public async Task<IList<string>> GetRolesAsync(string id)
    {
        var user = await GetUserCachedAsync(id);
        if (user is null) return new List<string>();
        return await _userManager.GetRolesAsync(user);
    }

    public async Task<IList<AppRole>> GetAppRolesAsync(string id) =>
        AppRole.RolesAsAppRoles(await GetRolesAsync(id)).OrderBy(r => r.DisplayName).ToList();

    public async Task<IdentityResult> UpdateRolesAsync(string id, Dictionary<string, bool> roles)
    {
        var user = await _userManager.FindByIdAsync(id)
            ?? throw new EntityNotFoundException(typeof(ApplicationUser), id);

        foreach (var (role, value) in roles)
        {
            var result = await UpdateUserRoleAsync(user, role, value);
            if (result != IdentityResult.Success) return result;
        }

        _cache.Set(id, user, TimeSpan.FromMinutes(UserExpirationMinutes));
        
        return IdentityResult.Success;

        async Task<IdentityResult> UpdateUserRoleAsync(ApplicationUser u, string r, bool addToRole)
        {
            var isInRole = await _userManager.IsInRoleAsync(u, r);
            if (addToRole == isInRole) return IdentityResult.Success;

            return addToRole switch
            {
                true => await _userManager.AddToRoleAsync(u, r),
                false => await _userManager.RemoveFromRoleAsync(u, r),
            };
        }
    }

    /// <summary>
    /// Asynchronously updates the information of a staff member identified by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the staff member to update.</param>
    /// <param name="resource">Update request containing the updated information.</param>
    /// <returns><see cref="IdentityResult"/> indicating the success or failure of the update operation.</returns>
    public async Task<IdentityResult> UpdateAsync(string id, StaffUpdateDto resource)
    {
        _cache.Remove(id);
        
        var user = await _userManager.FindByIdAsync(id)
            ?? throw new EntityNotFoundException(typeof(ApplicationUser), id);

        user.Phone = resource.Phone;
        user.Office = resource.OfficeId is null ? null : await _officeRepository.GetAsync(resource.OfficeId.Value);
        user.Active = resource.Active;

        return await _userManager.UpdateAsync(user);
    }

    public void Dispose() => _officeRepository.Dispose();
}
