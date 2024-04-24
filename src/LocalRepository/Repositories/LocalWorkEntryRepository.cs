using MyApp.Domain.Entities.EntryActions;
using MyApp.Domain.Entities.WorkEntries;
using MyApp.TestData;

namespace MyApp.LocalRepository.Repositories;

public sealed class LocalWorkEntryRepository(IEntryActionRepository entryActionRepository)
    : BaseRepository<WorkEntry>(WorkEntryData.GetData), IWorkEntryRepository
{
    public async Task<WorkEntry?> FindIncludeAllAsync(Guid id, bool includeDeletedActions = false,
        CancellationToken token = default) =>
        await GetWorkEntryDetailsAsync(await FindAsync(id, token).ConfigureAwait(false), includeDeletedActions, token)
            .ConfigureAwait(false);

    private async Task<WorkEntry?> GetWorkEntryDetailsAsync(WorkEntry? workEntry, bool includeDeletedActions,
        CancellationToken token)
    {
        if (workEntry is null) return null;

        workEntry.EntryActions.Clear();
        workEntry.EntryActions.AddRange((await entryActionRepository
                .GetListAsync(entryAction => entryAction.WorkEntry.Id == workEntry.Id &&
                                        (!entryAction.IsDeleted || includeDeletedActions), token)
                .ConfigureAwait(false))
            .OrderByDescending(entryAction => entryAction.ActionDate)
            .ThenBy(entryAction => entryAction.Id));

        return workEntry;
    }
}
