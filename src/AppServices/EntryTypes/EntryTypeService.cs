using AutoMapper;
using MyApp.AppServices.ServiceBase;
using MyApp.AppServices.UserServices;
using MyApp.Domain.Entities.EntryTypes;

namespace MyApp.AppServices.EntryTypes;

public sealed class EntryTypeService(
    IEntryTypeRepository repository,
    IEntryTypeManager manager,
    IMapper mapper,
    IUserService userService)
    : MaintenanceItemService<EntryType, EntryTypeViewDto, EntryTypeUpdateDto>
        (repository, manager, mapper, userService),
        IEntryTypeService;
