using GaEpd.AppLibrary.Pagination;
using MyApp.AppServices.Notifications;
using MyApp.AppServices.WorkEntries.CommandDto;
using MyApp.AppServices.WorkEntries.QueryDto;

namespace MyApp.AppServices.WorkEntries;

public interface IWorkEntryService : IDisposable, IAsyncDisposable
{
    Task<WorkEntryViewDto?> FindAsync(Guid id, bool includeDeletedActions = false, CancellationToken token = default);

    Task<WorkEntryUpdateDto?> FindForUpdateAsync(Guid id, CancellationToken token = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken token = default);

    Task<IPaginatedResult<WorkEntrySearchResultDto>> SearchAsync(WorkEntrySearchDto spec, PaginatedRequest paging,
        CancellationToken token = default);

    Task<WorkEntryCreateResult> CreateAsync(WorkEntryCreateDto resource, CancellationToken token = default);

    Task UpdateAsync(Guid id, WorkEntryUpdateDto resource, CancellationToken token = default);

    Task CloseAsync(WorkEntryCommentDto resource, string? baseUrl, CancellationToken token = default);

    Task<NotificationResult> ReopenAsync(WorkEntryCommentDto resource, string? baseUrl,
        CancellationToken token = default);

    Task DeleteAsync(WorkEntryCommentDto resource, CancellationToken token = default);

    Task RestoreAsync(WorkEntryCommentDto resource, CancellationToken token = default);
}
