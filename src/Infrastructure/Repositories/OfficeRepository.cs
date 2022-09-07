using Microsoft.EntityFrameworkCore;
using MyAppRoot.Domain.Entities;
using MyAppRoot.Domain.Offices;
using MyAppRoot.Infrastructure.Contexts;

namespace MyAppRoot.Infrastructure.Repositories;

public sealed class OfficeRepository : BaseRepository<Office, Guid>, IOfficeRepository
{
    public OfficeRepository(AppDbContext dbContext) : base(dbContext) { }

    public Task<Office?> FindByNameAsync(string name, CancellationToken token = default) =>
        DbContext.Offices.AsNoTracking().SingleOrDefaultAsync(e => e.Name == name, token);

    public async Task<List<ApplicationUser>>
        GetActiveStaffMembersListAsync(Guid id, CancellationToken token = default) =>
        (await GetAsync(id, token)).StaffMembers.Where(e => e.Active).ToList();
}
