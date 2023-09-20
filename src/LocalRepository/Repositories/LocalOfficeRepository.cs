using MyApp.Domain.Entities.Offices;
using MyApp.TestData;

namespace MyApp.LocalRepository.Repositories;

public sealed class LocalOfficeRepository : NamedEntityRepository<Office>, IOfficeRepository
{
    public LocalOfficeRepository() : base(OfficeData.GetOffices) { }
}
