using AutoMapper;
using GaEpd.AppLibrary.Pagination;
using MyApp.AppServices.Customers.Dto;
using MyApp.AppServices.Staff.Dto;
using MyApp.AppServices.UserServices;
using MyApp.Domain.Entities.Contacts;
using MyApp.Domain.Entities.Customers;
using MyApp.Domain.Identity;

namespace MyApp.AppServices.Customers;

public sealed class CustomerService : ICustomerService
{
    private readonly IMapper _mapper;
    private readonly IUserService _users;
    private readonly ICustomerRepository _customers;
    private readonly ICustomerManager _manager;
    private readonly IContactRepository _contacts;

    public CustomerService(
        IMapper mapper, IUserService users, ICustomerRepository customers,
        ICustomerManager manager, IContactRepository contacts)
    {
        _mapper = mapper;
        _users = users;
        _customers = customers;
        _manager = manager;
        _contacts = contacts;
    }

    // Customer read

    public async Task<IPaginatedResult<CustomerSearchResultDto>> SearchAsync(
        CustomerSearchDto spec, PaginatedRequest paging, CancellationToken token = default)
    {
        var predicate = CustomerFilters.CustomerSearchPredicate(spec);

        var count = await _customers.CountAsync(predicate, token);

        var list = count > 0
            ? _mapper.Map<List<CustomerSearchResultDto>>(await _customers.GetPagedListAsync(predicate, paging, token))
            : new List<CustomerSearchResultDto>();

        return new PaginatedResult<CustomerSearchResultDto>(list, count, paging);
    }

    public async Task<CustomerViewDto?> FindAsync(Guid id, CancellationToken token = default)
    {
        var customer = await _customers.FindIncludeAllAsync(id, token);
        if (customer is null) return null;

        var view = _mapper.Map<CustomerViewDto>(customer);
        return customer is { IsDeleted: true, DeletedById: not null }
            ? view with { DeletedBy = _mapper.Map<StaffViewDto>(await _users.FindUserAsync(customer.DeletedById)) }
            : view;
    }

    public async Task<CustomerSearchResultDto?> FindBasicInfoAsync(Guid id, CancellationToken token = default) =>
        _mapper.Map<CustomerSearchResultDto>(await _customers.FindAsync(id, token));

    // Customer write

    public async Task<Guid> CreateAsync(CustomerCreateDto resource, CancellationToken token = default)
    {
        var user = await _users.GetCurrentUserAsync();
        var customer = _manager.Create(resource.Name, user?.Id);

        customer.Description = resource.Description;
        customer.County = resource.County;
        customer.Website = resource.Website;
        customer.MailingAddress = resource.MailingAddress;

        await _customers.InsertAsync(customer, autoSave: false, token: token);
        await CreateContactAsync(customer, resource.Contact, user, token);

        await _customers.SaveChangesAsync(token);
        return customer.Id;
    }

    public async Task<CustomerUpdateDto?> FindForUpdateAsync(Guid id, CancellationToken token = default) =>
        _mapper.Map<CustomerUpdateDto>(await _customers.FindAsync(id, token));

    public async Task UpdateAsync(CustomerUpdateDto resource, CancellationToken token = default)
    {
        var item = await _customers.GetAsync(resource.Id, token);
        item.SetUpdater((await _users.GetCurrentUserAsync())?.Id);

        item.Name = resource.Name;
        item.Description = resource.Description;
        item.County = resource.County;
        item.MailingAddress = resource.MailingAddress;

        await _customers.UpdateAsync(item, token: token);
    }

    public async Task DeleteAsync(Guid id, string? deleteComments, CancellationToken token = default)
    {
        var item = await _customers.GetAsync(id, token);
        item.SetDeleted((await _users.GetCurrentUserAsync())?.Id);
        item.DeleteComments = deleteComments;
        await _customers.UpdateAsync(item, token: token);
    }

    public async Task RestoreAsync(Guid id, CancellationToken token = default)
    {
        var item = await _customers.GetAsync(id, token);
        item.SetNotDeleted();
        await _customers.UpdateAsync(item, token: token);
    }

    // Contacts

    public async Task<Guid> AddContactAsync(ContactCreateDto resource, CancellationToken token = default)
    {
        var customer = await _customers.GetAsync(resource.CustomerId, token);
        var id = await CreateContactAsync(customer, resource, await _users.GetCurrentUserAsync(), token);
        await _contacts.SaveChangesAsync(token);
        return id;
    }

    private async Task<Guid> CreateContactAsync(
        Customer customer, ContactCreateDto resource, ApplicationUser? user, CancellationToken token = default)
    {
        if (resource.IsEmpty) return Guid.Empty;

        var contact = _manager.CreateContact(customer, user?.Id);

        contact.Honorific = resource.Honorific;
        contact.GivenName = resource.GivenName;
        contact.FamilyName = resource.FamilyName;
        contact.Title = resource.Title;
        contact.Email = resource.Email;
        contact.Notes = resource.Notes;
        contact.Address = resource.Address;
        contact.EnteredBy = user;

        await _contacts.InsertAsync(contact, autoSave: false, token: token);
        return contact.Id;
    }

    public async Task<ContactViewDto?> FindContactAsync(Guid contactId, CancellationToken token = default) =>
        _mapper.Map<ContactViewDto>(await _contacts.FindAsync(e => e.Id == contactId && !e.IsDeleted, token));

    public async Task<ContactUpdateDto?> FindContactForUpdateAsync(Guid contactId, CancellationToken token = default) =>
        _mapper.Map<ContactUpdateDto>(await _contacts.FindAsync(e => e.Id == contactId && !e.IsDeleted, token));

    public async Task UpdateContactAsync(ContactUpdateDto resource, CancellationToken token = default)
    {
        var item = await _contacts.GetAsync(resource.Id, token);
        item.SetUpdater((await _users.GetCurrentUserAsync())?.Id);

        item.Honorific = resource.Honorific;
        item.GivenName = resource.GivenName;
        item.FamilyName = resource.FamilyName;
        item.Title = resource.Title;
        item.Email = resource.Email;
        item.Notes = resource.Notes;
        item.Address = resource.Address;

        await _contacts.UpdateAsync(item, token: token);
    }

    public async Task DeleteContactAsync(Guid contactId, CancellationToken token = default)
    {
        var item = await _contacts.GetAsync(contactId, token);
        item.SetDeleted((await _users.GetCurrentUserAsync())?.Id);
        await _contacts.UpdateAsync(item, token: token);
    }

    public void Dispose()
    {
        _customers.Dispose();
        _contacts.Dispose();
    }
}