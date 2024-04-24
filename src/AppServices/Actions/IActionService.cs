using MyApp.AppServices.Actions.Dto;

namespace MyApp.AppServices.Actions;

public interface IActionService : IDisposable, IAsyncDisposable
{
    Task<Guid> CreateAsync(ActionCreateDto resource, CancellationToken token = default);
    Task<ActionViewDto?> FindAsync(Guid id, CancellationToken token = default);
    Task<ActionUpdateDto?> FindForUpdateAsync(Guid id, CancellationToken token = default);
    Task UpdateAsync(Guid id, ActionUpdateDto resource, CancellationToken token = default);
    Task DeleteAsync(Guid actionItemId, CancellationToken token = default);
    Task RestoreAsync(Guid actionItemId, CancellationToken token = default);
}
