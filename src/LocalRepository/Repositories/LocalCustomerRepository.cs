using MyApp.Domain.Entities.Contacts;
using MyApp.Domain.Entities.Customers;
using MyApp.TestData;

namespace MyApp.LocalRepository.Repositories;

public sealed class LocalCustomerRepository : BaseRepository<Customer, Guid>, ICustomerRepository
{
    private readonly IContactRepository _contactRepository;

    public LocalCustomerRepository(IContactRepository contactRepository) : base(CustomerData.GetCustomers) =>
        _contactRepository = contactRepository;

    public async Task<Customer?> FindIncludeAllAsync(Guid id, CancellationToken token = default)
    {
        var result = await FindAsync(id, token);
        if (result is null) return result;

        result.Contacts = (await _contactRepository
                .GetListAsync(e => e.Customer.Id == id && !e.IsDeleted, token))
            .OrderByDescending(i => i.EnteredOn)
            .ToList();

        return result;
    }
}
