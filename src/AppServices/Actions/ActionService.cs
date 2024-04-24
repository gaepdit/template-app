using AutoMapper;
using MyApp.AppServices.Actions.Dto;
using MyApp.AppServices.UserServices;
using MyApp.Domain.Entities.WorkEntries;
using MyApp.Domain.Entities.WorkEntryActions;

namespace MyApp.AppServices.Actions;

public sealed class ActionService(
    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    IMapper mapper,
    IUserService userService,
    IWorkEntryRepository workEntryRepository,
    IWorkEntryManager workEntryManager,
    IActionRepository actionRepository)
    : IActionService
{
    public async Task<Guid> CreateAsync(ActionCreateDto resource, CancellationToken token = default)
    {
        var workEntry = await workEntryRepository.GetAsync(resource.WorkEntryId, token).ConfigureAwait(false);
        var currentUser = await userService.GetCurrentUserAsync().ConfigureAwait(false);
        var action = workEntryManager.CreateAction(workEntry, currentUser);

        action.ActionDate = resource.ActionDate!.Value;
        action.Comments = resource.Comments;

        await actionRepository.InsertAsync(action, token: token).ConfigureAwait(false);
        return action.Id;
    }

    public async Task<ActionViewDto?> FindAsync(Guid id, CancellationToken token = default) =>
        mapper.Map<ActionViewDto>(
            await actionRepository.FindAsync(id, token).ConfigureAwait(false));

    public async Task<ActionUpdateDto?> FindForUpdateAsync(Guid id, CancellationToken token = default) =>
        mapper.Map<ActionUpdateDto>(
            await actionRepository.FindAsync(action => action.Id == id && !action.IsDeleted, token)
                .ConfigureAwait(false));

    public async Task UpdateAsync(Guid id, ActionUpdateDto resource, CancellationToken token = default)
    {
        var action = await actionRepository.GetAsync(id, token).ConfigureAwait(false);
        action.SetUpdater((await userService.GetCurrentUserAsync().ConfigureAwait(false))?.Id);

        action.ActionDate = resource.ActionDate!.Value;
        action.Comments = resource.Comments;

        await actionRepository.UpdateAsync(action, token: token).ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid actionItemId, CancellationToken token = default)
    {
        var action = await actionRepository.GetAsync(actionItemId, token).ConfigureAwait(false);
        action.SetDeleted((await userService.GetCurrentUserAsync().ConfigureAwait(false))?.Id);
        await actionRepository.UpdateAsync(action, token: token).ConfigureAwait(false);
    }

    public async Task RestoreAsync(Guid actionItemId, CancellationToken token = default)
    {
        var action = await actionRepository.GetAsync(actionItemId, token).ConfigureAwait(false);
        action.SetNotDeleted();
        await actionRepository.UpdateAsync(action, token: token).ConfigureAwait(false);
    }

    public void Dispose()
    {
        workEntryRepository.Dispose();
        actionRepository.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await workEntryRepository.DisposeAsync().ConfigureAwait(false);
        await actionRepository.DisposeAsync().ConfigureAwait(false);
    }
}
