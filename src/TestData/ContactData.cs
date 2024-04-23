using MyApp.Domain.Entities.Contacts;
using MyApp.Domain.ValueObjects;
using MyApp.TestData.Constants;
using MyApp.TestData.Identity;

namespace MyApp.TestData;

internal static class ContactData
{
    private static IEnumerable<Contact> ContactSeedItems() => new List<Contact>
    {
        new(new Guid("41000000-0000-0000-0000-000000000001"),
            CustomerData.GetCustomers.ElementAt(0))
        {
            EnteredBy = UserData.GetUsers.ElementAt(1),
            EnteredDate = DateTimeOffset.Now.AddDays(-5),
            Honorific = "Mr.",
            GivenName = TextData.Word,
            FamilyName = TextData.AnotherWord,
            Title = TextData.ShortPhrase,
            Email = TextData.ValidEmail,
            Notes = TextData.MultipleParagraphs,
            Address = ValueObjectData.CompleteAddress,
        },
        new(new Guid("41000000-0000-0000-0000-000000000002"),
            CustomerData.GetCustomers.ElementAt(0))
        {
            EnteredBy = UserData.GetUsers.ElementAt(0),
            EnteredDate = DateTimeOffset.Now.AddDays(-4),
            Honorific = string.Empty,
            GivenName = string.Empty,
            FamilyName = string.Empty,
            Title = TextData.AnotherShortPhrase,
            Email = string.Empty,
            Notes = string.Empty,
            Address = IncompleteAddress.EmptyAddress,
        },
        new(new Guid("41000000-0000-0000-0000-000000000003"),
            CustomerData.GetCustomers.ElementAt(0))
        {
            EnteredBy = UserData.GetUsers.ElementAt(3),
            EnteredDate = DateTimeOffset.Now.AddDays(-3),
            Honorific = "Ms.",
            GivenName = "Deleted",
            FamilyName = "Contact",
            Title = TextData.Phrase,
            Email = TextData.ValidEmail,
            Notes = TextData.Paragraph,
            Address = ValueObjectData.IncompleteAddress,
        },
        new(new Guid("41000000-0000-0000-0000-000000000004"),
            CustomerData.GetCustomers.ElementAt(2))
        {
            EnteredBy = UserData.GetUsers.ElementAt(1),
            EnteredDate = DateTimeOffset.Now.AddDays(-2),
            Honorific = "Mx.",
            GivenName = TextData.AnotherWord,
            FamilyName = TextData.EmojiWord,
            Title = TextData.Phrase,
            Email = TextData.ValidEmail,
            Notes = TextData.Paragraph,
            Address = ValueObjectData.IncompleteAddress,
        },
    };

    private static IEnumerable<Contact>? _contacts;

    public static IEnumerable<Contact> GetContacts()
    {
        if (_contacts is not null) return _contacts;

        _contacts = ContactSeedItems().ToList();
        _contacts.ElementAt(2).SetDeleted("00000000-0000-0000-0000-000000000001");
        return _contacts;
    }

    public static void ClearData() => _contacts = null;
}
