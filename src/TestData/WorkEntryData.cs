using MyApp.Domain.Entities.WorkEntries;
using MyApp.TestData.Constants;
using MyApp.TestData.Identity;

namespace MyApp.TestData;

internal static class WorkEntryData
{
    private static IEnumerable<WorkEntry> WorkEntrySeedItems => new List<WorkEntry>
    {
        new(new Guid("10000000-0000-0000-0000-000000000000")) // 0
        {
            Status = WorkEntryStatus.Closed,
            EnteredBy = UserData.GetUsers.ElementAt(0),
            Notes = TextData.Paragraph,
        },
        new(new Guid("10000000-0000-0000-0000-000000000001")) // 1
        {
            Status = WorkEntryStatus.Open,
            EnteredBy = UserData.GetUsers.ElementAt(1),
            EnteredDate = DateTimeOffset.Now.AddMinutes(30),
        },
        new(new Guid("10000000-0000-0000-0000-000000000002")) // 2
        {
            Status = WorkEntryStatus.Closed,
            EnteredBy = UserData.GetUsers.ElementAt(2),
        },
        new(new Guid("10000000-0000-0000-0000-000000000003")) // 3
        {
            Notes = "Deleted work entry",
            Status = WorkEntryStatus.Closed,
            EnteredBy = UserData.GetUsers.ElementAt(0),
            DeleteComments = TextData.Paragraph,
        },
        new(new Guid("10000000-0000-0000-0000-000000000004")) // 4
        {
            Status = WorkEntryStatus.Open,
            EnteredBy = UserData.GetUsers.ElementAt(1),
        },
        new(new Guid("10000000-0000-0000-0000-000000000005")) // 5
        {
            Status = WorkEntryStatus.Open,
            EnteredBy = UserData.GetUsers.ElementAt(1),
        },
        new(new Guid("10000000-0000-0000-0000-000000000006")) // 6
        {
            Notes = "Open WorkEntry assigned to inactive user.",
            Status = WorkEntryStatus.Open,
            EnteredBy = UserData.GetUsers.ElementAt(3),
        },
    };

    private static IEnumerable<WorkEntry>? _workEntries;

    public static IEnumerable<WorkEntry> GetWorkEntries
    {
        get
        {
            if (_workEntries is not null) return _workEntries;

            _workEntries = WorkEntrySeedItems.ToList();
            _workEntries.ElementAt(3).SetDeleted("00000000-0000-0000-0000-000000000001");

            return _workEntries;
        }
    }

    public static void ClearData() => _workEntries = null;
}
