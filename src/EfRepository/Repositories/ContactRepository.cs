using MyApp.Domain.Entities.Contacts;

namespace MyApp.EfRepository.Repositories;

public sealed class ContactRepository : BaseRepository<Contact, Guid, AppDbContext>, IContactRepository
{
    public ContactRepository(AppDbContext context) : base(context) { }
}
