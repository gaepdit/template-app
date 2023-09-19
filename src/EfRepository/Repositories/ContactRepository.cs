using MyApp.Domain.Entities.Contacts;

namespace MyApp.EfRepository.Repositories;

public sealed class ContactRepository : BaseRepository<Contact, Guid>, IContactRepository
{
    public ContactRepository(DbContext context) : base(context) { }
}
