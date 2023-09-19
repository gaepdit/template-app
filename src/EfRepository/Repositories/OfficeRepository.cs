using MyApp.Domain.Entities.Offices;

namespace MyApp.EfRepository.Repositories;

public sealed class OfficeRepository : NamedEntityRepository<Office>, IOfficeRepository
{
    public OfficeRepository(DbContext context) : base(context) { }
}
