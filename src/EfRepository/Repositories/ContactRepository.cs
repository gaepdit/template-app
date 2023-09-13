using MyApp.Domain.Entities.Contacts;
using MyApp.EfRepository.Contexts;

namespace MyApp.EfRepository.Repositories;

public sealed class ContactRepository : BaseRepository<Contact, Guid>, IContactRepository
{
    public ContactRepository(AppDbContext context) : base(context) { }
}
