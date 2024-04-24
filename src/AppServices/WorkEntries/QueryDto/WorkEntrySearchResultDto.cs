using MyApp.Domain.Entities.WorkEntries;

namespace MyApp.AppServices.WorkEntries.QueryDto;

public record WorkEntrySearchResultDto
{
    public Guid Id { get; init; }
    public DateTimeOffset ReceivedDate { get; init; }
    public WorkEntryStatus Status { get; init; }
    public bool WorkEntryClosed { get; init; }
    public DateTimeOffset? WorkEntryClosedDate { get; init; }
    public string? EntryTypeName { get; init; }
    public bool IsDeleted { get; init; }
}
