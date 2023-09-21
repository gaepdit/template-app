using MyApp.Domain.Entities.Offices;

namespace MyApp.EfRepository.Repositories;

public sealed class OfficeRepository : NamedEntityRepository<Office, AppDbContext>, IOfficeRepository
{
    public OfficeRepository(AppDbContext context) : base(context) { }
}
