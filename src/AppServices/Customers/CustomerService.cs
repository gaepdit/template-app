using AutoMapper;
using GaEpd.AppLibrary.Pagination;
using Microsoft.Extensions.Caching.Memory;
using MyApp.AppServices.Customers.Dto;
using MyApp.AppServices.Staff.Dto;
using MyApp.AppServices.UserServices;
using MyApp.Domain.Entities.Contacts;
using MyApp.Domain.Entities.Customers;
using MyApp.Domain.Identity;

namespace MyApp.AppServices.Customers;

public sealed class CustomerService : ICustomerService
{
    private const double CustomerExpirationMinutes = 5.0;
    
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly ICustomerRepository _customerRepository;
    private readonly ICustomerManager _customerManager;
    private readonly IContactRepository _contactRepository;
    private readonly IMemoryCache _cache;

    public CustomerService(
        IMapper mapper, IUserService userService, ICustomerRepository customerRepository,
        ICustomerManager customerManager, IContactRepository contactRepository, IMemoryCache cache)
    {
        _mapper = mapper;
        _userService = userService;
        _customerRepository = customerRepository;
        _customerManager = customerManager;
        _contactRepository = contactRepository;
        _cache = cache;
    }

    // Customer read
    
    public async Task<IPaginatedResult<CustomerSearchResultDto>> SearchAsync(
        CustomerSearchDto spec, PaginatedRequest paging, CancellationToken token = default)
    {
        var predicate = CustomerFilters.CustomerSearchPredicate(spec);

        var count = await _customerRepository.CountAsync(predicate, token);

        var list = count > 0
            ? _mapper.Map<List<CustomerSearchResultDto>>(
                await _customerRepository.GetPagedListAsync(predicate, paging, token))
            : new List<CustomerSearchResultDto>();
        
        // TODO #1 - In second thought this should be saved based on the search params.
        // if(list.Count > 0) list.ForEach(item =>
        // {
        //     if(_cache.Get(item.Id) is null) 
        //         _cache.Set(item.Id, item, TimeSpan.FromMinutes(CustomerExpirationMinutes));
        // });
        
        return new PaginatedResult<CustomerSearchResultDto>(list, count, paging);
    }
    
    /// <summary>
    /// Private method used to asynchronously retrieve <see cref="Customer"/> based on its unique identifier.
    /// </summary>
    /// <param name="id">Customer's unique identifier.</param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns>The <see cref="Customer"/> with the provided identifier if present and null otherwise</returns>
    private async Task<Customer?> GetCustomerCachedAsync(Guid id, CancellationToken token = default)
    {
        var customer = _cache.Get<Customer>(id);
        if (customer is null)
        {
            customer = await _customerRepository.FindIncludeAllAsync(id, token);
            if (customer is null) return null;

            _cache.Set(id, customer, TimeSpan.FromMinutes(CustomerExpirationMinutes));
        }
        return customer;
    }

    /// <summary>
    /// Asynchronously retrieves a specific customer based on its unique identifier.
    /// </summary>
    /// <param name="id">Customer's unique identifier.</param>
    /// <param name="token"><see cref="CancellationToken"/></param>
    /// <returns><see cref="CustomerViewDto"/> associated with the required customer if present and null otherwise.</returns>
    public async Task<CustomerViewDto?> FindAsync(Guid id, CancellationToken token = default)
    {
        var customer = await GetCustomerCachedAsync(id, token);
        if (customer is null) return null;

        var view = _mapper.Map<CustomerViewDto>(customer);
        return customer is { IsDeleted: true, DeletedById: not null }
            ? view with
            {
                DeletedBy = _mapper.Map<StaffViewDto>(await _userService.FindUserAsync(customer.DeletedById))
            }
            : view;
    }

    public async Task<CustomerSearchResultDto?> FindBasicInfoAsync(Guid id, CancellationToken token = default) => 
        _mapper.Map<CustomerSearchResultDto>(await GetCustomerCachedAsync(id, token));

    // Customer write

    public async Task<Guid> CreateAsync(CustomerCreateDto resource, CancellationToken token = default)
    {
        var user = await _userService.GetCurrentUserAsync();
        var customer = _customerManager.Create(resource.Name, user?.Id);

        customer.Description = resource.Description;
        customer.County = resource.County;
        customer.Website = resource.Website;
        customer.MailingAddress = resource.MailingAddress;

        await _customerRepository.InsertAsync(customer, autoSave: false, token: token);
        await CreateContactAsync(customer, resource.Contact, user, token);

        await _customerRepository.SaveChangesAsync(token);
        return customer.Id;
    }

    public async Task<CustomerUpdateDto?> FindForUpdateAsync(Guid id, CancellationToken token = default) =>
        _mapper.Map<CustomerUpdateDto>(await GetCustomerCachedAsync(id, token));

    public async Task UpdateAsync(Guid id, CustomerUpdateDto resource, CancellationToken token = default)
    {
        _cache.Remove(id);
        
        var item = await _customerRepository.GetAsync(id, token);
        item.SetUpdater((await _userService.GetCurrentUserAsync())?.Id);

        item.Name = resource.Name;
        item.Description = resource.Description;
        item.County = resource.County;
        item.MailingAddress = resource.MailingAddress;

        await _customerRepository.UpdateAsync(item, token: token);
    }

    public async Task DeleteAsync(Guid id, string? deleteComments, CancellationToken token = default)
    {
        var item = await _customerRepository.GetAsync(id, token);
        item.SetDeleted((await _userService.GetCurrentUserAsync())?.Id);
        item.DeleteComments = deleteComments;
        await _customerRepository.UpdateAsync(item, token: token);
    } //TODO #2 - in PR ask if that should be cached here too? deletion doesn't make sense to cache here.

    public async Task RestoreAsync(Guid id, CancellationToken token = default)
    {
        var item = await GetCustomerCachedAsync(id, token);
        /* TODO #3 - at PR, previously this method did not expect item to be null. If this was a mistake,
         * consider throwing/ returning appropriate message. Right now it just to stop IDE error.
         */
        if (item is null) return;
        item.SetNotDeleted();
        await _customerRepository.UpdateAsync(item, token: token);
    }

    // Contacts
    //TODO - ask about caching those. remove before PR

    public async Task<Guid> AddContactAsync(ContactCreateDto resource, CancellationToken token = default)
    {
        var customer = await GetCustomerCachedAsync(resource.CustomerId, token);
        // ! -> same as #3.
        var id = await CreateContactAsync(customer!, resource, await _userService.GetCurrentUserAsync(), token);
        await _contactRepository.SaveChangesAsync(token);
        
        return id;
    }

    private async Task<Guid> CreateContactAsync(
        Customer customer, ContactCreateDto resource, ApplicationUser? user, CancellationToken token = default)
    {
        if (resource.IsEmpty) return Guid.Empty;

        var contact = _customerManager.CreateContact(customer, user?.Id);

        contact.Honorific = resource.Honorific;
        contact.GivenName = resource.GivenName;
        contact.FamilyName = resource.FamilyName;
        contact.Title = resource.Title;
        contact.Email = resource.Email;
        contact.Notes = resource.Notes;
        contact.Address = resource.Address;
        contact.EnteredBy = user;

        await _contactRepository.InsertAsync(contact, autoSave: false, token: token);
        return contact.Id;
    }

    public async Task<ContactViewDto?> FindContactAsync(Guid contactId, CancellationToken token = default) =>
        _mapper.Map<ContactViewDto>(await _contactRepository.FindAsync(e => e.Id == contactId && !e.IsDeleted, token));

    public async Task<ContactUpdateDto?> FindContactForUpdateAsync(Guid contactId, CancellationToken token = default) =>
        _mapper.Map<ContactUpdateDto>(
            await _contactRepository.FindAsync(e => e.Id == contactId && !e.IsDeleted, token));

    public async Task UpdateContactAsync(Guid contactId, ContactUpdateDto resource, CancellationToken token = default)
    {
        var item = await _contactRepository.GetAsync(contactId, token);
        item.SetUpdater((await _userService.GetCurrentUserAsync())?.Id);

        item.Honorific = resource.Honorific;
        item.GivenName = resource.GivenName;
        item.FamilyName = resource.FamilyName;
        item.Title = resource.Title;
        item.Email = resource.Email;
        item.Notes = resource.Notes;
        item.Address = resource.Address;

        await _contactRepository.UpdateAsync(item, token: token);
    }

    public async Task DeleteContactAsync(Guid contactId, CancellationToken token = default)
    {
        var item = await _contactRepository.GetAsync(contactId, token);
        item.SetDeleted((await _userService.GetCurrentUserAsync())?.Id);
        await _contactRepository.UpdateAsync(item, token: token);
    }

    public void Dispose()
    {
        _customerRepository.Dispose();
        _contactRepository.Dispose();
    }
}
