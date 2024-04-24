using GaEpd.AppLibrary.ListItems;
using MyApp.AppServices.ServiceBase;

namespace MyApp.AppServices.Offices;

public interface IOfficeService : IMaintenanceItemService<OfficeViewDto, OfficeUpdateDto>
{
    Task<OfficeViewDto?> FindAsync(Guid id, CancellationToken token = default);
    Task<IReadOnlyList<OfficeViewDto>> GetListIncludeAssignorAsync(CancellationToken token = default);
    Task<Guid> CreateAsync(OfficeCreateDto resource, CancellationToken token = default);

    Task<IReadOnlyList<ListItem<string>>> GetStaffAsListItemsAsync(Guid? id, bool includeInactive = false,
        CancellationToken token = default);
}
