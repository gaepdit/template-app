using MyApp.Domain.Identity;

namespace MyApp.Domain.Entities.WorkEntries;

public class WorkEntryManager(IWorkEntryRepository repository) : IWorkEntryManager
{
    // Complaints
    public WorkEntry Create(ApplicationUser? user)
    {
        var item = new WorkEntry(Guid.NewGuid()) { EnteredBy = user };
        item.SetCreator(user?.Id);
        return item;
    }

    public void Close(WorkEntry workEntry, string? comment, ApplicationUser? user)
    {
        workEntry.SetUpdater(user?.Id);
        workEntry.Status = WorkEntryStatus.Closed;
        workEntry.Closed = true;
        workEntry.ClosedDate = DateTime.Now;
        workEntry.ClosedBy = user;
        workEntry.ClosedComments = comment;
    }

    public void Reopen(WorkEntry workEntry, ApplicationUser? user)
    {
        workEntry.SetUpdater(user?.Id);
        workEntry.Status = WorkEntryStatus.Open;
        workEntry.Closed = false;
        workEntry.ClosedDate = null;
        workEntry.ClosedBy = null;
        workEntry.ClosedComments = null;
    }

    public void Delete(WorkEntry workEntry, string? comment, ApplicationUser? user)
    {
        workEntry.SetDeleted(user?.Id);
        workEntry.DeletedBy = user;
        workEntry.DeleteComments = comment;
    }

    public void Restore(WorkEntry workEntry, ApplicationUser? user)
    {
        workEntry.SetNotDeleted();
        workEntry.DeleteComments = null;
    }
}
