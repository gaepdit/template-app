using AutoMapper;
using GaEpd.AppLibrary.ListItems;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using MyApp.AppServices.UserServices;
using MyApp.Domain.Entities.Offices;

namespace MyApp.AppServices.Offices;

public sealed class OfficeService : IOfficeService
{
    private const double OfficeExpirationMinutes = 5.0;
    
    private readonly IOfficeRepository _repository;
    private readonly IOfficeManager _manager;
    private readonly IUserService _users;
    private readonly IMemoryCache _cache;
    private readonly IMapper _mapper;

    public OfficeService(
        IOfficeRepository repository,
        IOfficeManager manager,
        IMapper mapper,
        IUserService users,
        IMemoryCache cache)
    {
        _repository = repository;
        _manager = manager;
        _mapper = mapper;
        _users = users;
        _cache = cache;
    }

    /// <summary>
    /// Asynchronously retrieves <see cref="Office"/> associated with the provided identifier.
    /// Caches it accordingly.
    /// </summary>
    /// <param name="id">Office's unique identifier.</param>
    /// <param name="token"><see cref="CancellationToken"/> (Optional).</param>
    /// <returns>Office object associated with the provided identifier if present and null otherwise.</returns>
    private async Task<Office?> GetOfficeCachedAsync(Guid id, CancellationToken token = default)
    {
        var office = _cache.Get<Office>(id);
        if (office is null)
        {
            office = await _repository.FindAsync(id, token);
            if (office is null) return null;

            _cache.Set(office.Id, office, TimeSpan.FromMinutes(OfficeExpirationMinutes));
        }
        return office.Active ? office : null;
    }
    
    public async Task<IReadOnlyList<OfficeViewDto>> GetListAsync(CancellationToken token = default)
    {
        var list = (await _repository.GetListAsync(token)).OrderBy(e => e.Name).ToList();
        return _mapper.Map<IReadOnlyList<OfficeViewDto>>(list);
    }

    public async Task<IReadOnlyList<ListItem>> GetActiveListItemsAsync(CancellationToken token = default) =>
        (await _repository.GetListAsync(e => e.Active, token)).OrderBy(e => e.Name)
        .Select(e => new ListItem(e.Id, e.Name)).ToList();

    /// <summary>
    /// Creates new <see cref="Office"/> object and persists it.
    /// </summary>
    /// <param name="resource">create request.</param>
    /// <param name="token"><see cref="CancellationToken"/> (Optional).</param>
    /// <returns>Identifier of the newly created office object.</returns>
    public async Task<Guid> CreateAsync(OfficeCreateDto resource, CancellationToken token = default)
    {
        var item = await _manager.CreateAsync(resource.Name, (await _users.GetCurrentUserAsync())?.Id, token);
        await _repository.InsertAsync(item, token: token);

        _cache.Set(item.Id, item, TimeSpan.FromMinutes(OfficeExpirationMinutes));
        
        return item.Id;
    }

    /// <summary>
    /// Asynchronously retrieves office associated with the provided identifier for update purposes. 
    /// </summary>
    /// <param name="id">Office's unique identifier.</param>
    /// <param name="token"><see cref="CancellationToken"/> (Optional).</param>
    /// <returns>
    /// <see cref="OfficeUpdateDto"/>
    /// </returns>
    public async Task<OfficeUpdateDto?> FindForUpdateAsync(Guid id, CancellationToken token = default)
    {
        var item = await GetOfficeCachedAsync(id, token);
        return _mapper.Map<OfficeUpdateDto>(item);
    }

    public async Task UpdateAsync(Guid id, OfficeUpdateDto resource, CancellationToken token = default)
    {
        _cache.Remove(id);
        
        var item = await _repository.GetAsync(id, token);
        item.SetUpdater((await _users.GetCurrentUserAsync())?.Id);

        if (item.Name != resource.Name.Trim())
            await _manager.ChangeNameAsync(item, resource.Name, token);
        item.Active = resource.Active;

        await _repository.UpdateAsync(item, token: token);
    }

    public void Dispose() => _repository.Dispose();
}
