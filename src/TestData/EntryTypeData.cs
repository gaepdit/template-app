using MyApp.Domain.Entities.EntryTypes;

namespace MyApp.TestData;

internal static class EntryTypeData
{
    private static IEnumerable<EntryType> EntryTypeSeedItems => new List<EntryType>
    {
        new(new Guid("20000000-0000-0000-0000-000000000001"), "Entry Type One"),
        new(new Guid("20000000-0000-0000-0000-000000000002"), "Entry Type Two"),
        new(new Guid("20000000-0000-0000-0000-000000000003"), "Entry Type Three"),
        new(new Guid("20000000-0000-0000-0000-000000000004"), "Entry Type Four"),
    };

    private static IEnumerable<EntryType>? _entryTypes;

    public static IEnumerable<EntryType> GetEntryTypes
    {
        get
        {
            if (_entryTypes is not null) return _entryTypes;
            _entryTypes = EntryTypeSeedItems;
            return _entryTypes;
        }
    }

    public static void ClearData() => _entryTypes = null;
}
