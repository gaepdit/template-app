using Microsoft.EntityFrameworkCore;
using MyApp.Domain.Entities.Customers;
using MyApp.EfRepository.Contexts;

namespace MyApp.EfRepository.Repositories;

public sealed class CustomerRepository : BaseRepository<Customer, Guid>, ICustomerRepository
{
    public CustomerRepository(AppDbContext context) : base(context) { }

    public async Task<Customer?> FindIncludeAllAsync(Guid id, CancellationToken token = default) =>
        await Context.Set<Customer>()
            .Include(e => e.Contacts
                .Where(i => !i.IsDeleted)
                .OrderByDescending(i => i.EnteredOn))
            .ThenInclude(e => e.EnteredBy)
            .SingleOrDefaultAsync(e => e.Id.Equals(id), token);
}
