using MyApp.Domain.Entities.Contacts;
using MyApp.TestData;

namespace MyApp.LocalRepository.Repositories;

public sealed class LocalContactRepository : BaseRepository<Contact, Guid>, IContactRepository
{
    public LocalContactRepository() : base(ContactData.GetContacts()) { }
}
