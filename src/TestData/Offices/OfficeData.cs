using MyAppRoot.Domain.Entities;

namespace MyAppRoot.TestData.Offices;

internal static class OfficeData
{
    private static List<Office> OfficeSeedItems() =>
        new()
        {
            new Office(Guid.NewGuid(), "Branch") { Active = true },
            new Office(Guid.NewGuid(), "District") { Active = true },
            new Office(Guid.NewGuid(), "Region") { Active = true },
            new Office(Guid.NewGuid(), "Closed Office") { Active = false },
        };

    private static ICollection<Office>? _offices;

    public static IEnumerable<Office> GetOffices
    {
        get
        {
            if (_offices is not null) return _offices;

            // Seed offices and user data.
            _offices = OfficeSeedItems();
            Identity.Data.GetUsers.First().Office = _offices.First();

            return _offices;
        }
    }
}
