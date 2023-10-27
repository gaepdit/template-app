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
    //Change those if needed:
    private const double CustomerExpirationMinutes = 5.0;
    private const double ContactExpirationMinutes = 5.0;
    
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
        
        return new PaginatedResult<CustomerSearchResultDto>(list, count, paging);
    }
    
    /// <summary>
    /// Private method used to asynchronously retrieve <see cref="Customer"/> based on its unique identifier.
    /// </summary>
    /// <param name="id">Customer's unique identifier.</param>
    /// <param name="token"><see cref="CancellationToken"/> (Optional)</param>
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
    /// <param name="token"><see cref="CancellationToken"/> (Optional)</param>
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

    /// <summary>
    /// Asynchronously retrieves information about a customer identified by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the customer desired.</param>
    /// <param name="token"><see cref="CancellationToken"/> (Optional)</param>
    /// <returns>
    /// DTO containing basic information about the customer, or null if the customer is not found.
    /// </returns>
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

        _cache.Set(customer.Id, customer, TimeSpan.FromMinutes(CustomerExpirationMinutes));
        
        await _customerRepository.SaveChangesAsync(token);
        return customer.Id;
    }
    
    /// <summary>
    /// Asynchronously retrieves customer for updating purposes by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the customer desired.</param>
    /// <param name="token"><see cref="CancellationToken"/> (Optional)</param>
    /// <returns>
    /// DTO with the data of the user as it currently is, meant to be updated, if not found will return null.
    /// </returns>
    public async Task<CustomerUpdateDto?> FindForUpdateAsync(Guid id, CancellationToken token = default) =>
        _mapper.Map<CustomerUpdateDto>(await GetCustomerCachedAsync(id, token));

    /// <summary>
    /// Asynchronously updates a customer identified by their unique identifier using the provided data in the resource.
    /// </summary>
    /// <param name="id">Customer's unique identifier.</param>
    /// <param name="resource">Updated data resource of the specified customer.</param>
    /// <param name="token"><see cref="CancellationToken"/> (Optional)</param>
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

    /// <summary>
    /// Asynchronously restores a previously deleted customer identified by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the customer to restore.</param>
    /// <param name="token"><see cref="CancellationToken"/> (Optional)</param>
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

        _cache.Set(contact.Id, contact, TimeSpan.FromMinutes(ContactExpirationMinutes));

        await _contactRepository.InsertAsync(contact, autoSave: false, token: token);
        return contact.Id;
    }

    /// <summary>
    /// Asynchronously retrieves the <see cref="Contact"/> associated with the provided identifier.
    /// Caches it accordingly.
    /// </summary>
    /// <param name="contactId">Contact's unique identifier.</param>
    /// <param name="token"><see cref="CancellationToken"/> (Optional)</param>
    /// <returns>Contact object associated with the provided identifier if present and null otherwise.</returns>
    private async Task<Contact?> GetContactCachedAsync(Guid contactId, CancellationToken token = default)
    {
        var contact = _cache.Get<Contact>(contactId);
        if (contact is null)
        {
            contact = await _contactRepository.FindAsync(e => e.Id == contactId && !e.IsDeleted, token);
            if (contact is null) return null;
            _cache.Set(contact.Id, contact, TimeSpan.FromMinutes(ContactExpirationMinutes));
        }
        return contact.IsDeleted ? null : contact;
    }
    
    /// <summary>
    /// Asynchronously retrieves Contact for display purposes.
    /// </summary>
    /// <param name="contactId">Contact's unique identifier.</param>
    /// <param name="token"><see cref="CancellationToken"/> (Optional)</param>
    /// <returns>
    /// <see cref="ContactViewDto"/>.
    /// </returns>
    public async Task<ContactViewDto?> FindContactAsync(Guid contactId, CancellationToken token = default) =>
        _mapper.Map<ContactViewDto>(await GetContactCachedAsync(contactId, token));

    /// <summary>
    /// Asynchronously retrieves Contact for updating purposes.
    /// </summary>
    /// <param name="contactId">Contact's unique identifier.</param>
    /// <param name="token"><see cref="CancellationToken"/> (Optional)</param>
    /// <returns>
    /// <see cref="ContactUpdateDto"/>.
    /// </returns>
    public async Task<ContactUpdateDto?> FindContactForUpdateAsync(Guid contactId, CancellationToken token = default) =>
        _mapper.Map<ContactUpdateDto>(await GetContactCachedAsync(contactId, token));

    public async Task UpdateContactAsync(Guid contactId, ContactUpdateDto resource, CancellationToken token = default)
    {
        _cache.Remove(contactId);
        
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
        _cache.Remove(contactId);
        
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
